using System;

namespace Liam
{
   /// <summary>
   /// Representa la la referencia de una propiedad hacia un objeto para profundizar en sus datos.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public class ObjetoExtensionAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece la lista de nombres de las propiedades para la lectura del objeto.
      /// </summary>
      public string[] Nombres { get; private set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ObjetoExtensionAttribute"/>.
      /// </summary>
      /// <param name="nombres">nombres de las propiedades para la lectura del objeto.</param>
      public ObjetoExtensionAttribute(params string[] nombres)
      {
         this.Nombres = nombres;
      }
   }

   public static class ComponenteRegistro
   {
      private static readonly lst.Dictionary<Type, ComponenteInformacion> _cache
         = new lst.Dictionary<Type, ComponenteInformacion>();

      public static void Inicializar(rfl.Assembly[] ensamblados = null)
      {
         var assemblies = (ensamblados == null || ensamblados.Length == 0)
            ? new[] { rfl.Assembly.GetExecutingAssembly() }
            : ensamblados;

         foreach (var assembly in assemblies)
         {
            var tipos = assembly.GetTypes()
               .Where(t =>
                  !t.IsAbstract &&
                  t.IsSubclassOf(typeof(Componente)) &&
                  t.GetCustomAttribute<ModeloAttribute>() != null);

            foreach (var tipo in tipos)
            {
               var info = ComponenteInformacion.Obtener(tipo);

               if (info.Claves.Count == 0)
                  throw new InvalidOperationException($"{tipo.Name} tiene [Modelo] pero no [Clave].");
               _cache[tipo] = info;
            }
         }
      }
   }

   public class ComponenteValidador
   {
      public bool Activo { get; set; } = true;

      public lst.List<string> Validar<T>(T instancia)
          where T : Componente
      {
         var errores = new lst.List<string>();
         if (!Activo) return errores;

         var info = ComponenteInformacion.Obtener<T>();
         foreach (var cp in info.Claves)
         {
            var valor = cp.ObtenerValor(instancia);
            if (valor == null || valor.Equals(ObtenerDefault(cp.Propiedad.PropertyType)))
               errores.Add($"Clave '{cp.NombreCampo}' no puede ser nula o vacía.");
         }
         return errores;
      }

      private object ObtenerDefault(Type tipo)
      {
         return tipo.IsValueType ? Activator.CreateInstance(tipo) : null;
      }
   }

   public class ComponenteOpciones
   {
      public int MaxFilas { get; set; } = 1000;
      public int MaxGrillas { get; set; } = 16;
      public thr.CancellationToken Cancelacion { get; set; } = thr.CancellationToken.None;

      public static ComponenteOpciones Predeterminado => new ComponenteOpciones();
   }

   internal static class ComponenteConstructor
   {
      public static string ArmarSelect<T>(object[] valores)
          where T : Componente
      {
         var info = ComponenteInfo.Obtener<T>();
         var campos = string.Join(", ", info.Propiedades.Select(p => p.NombreCampo));
         var where = string.Join(" AND ", info.Claves.Select(p => $"{p.NombreCampo} = {p.NombreParametro}"));
         return $"SELECT {campos} FROM {info.TablaCompleta} WHERE {where}";
      }

      public static string ArmarInsert<T>()
          where T : Componente
      {
         var info = ComponenteInfo.Obtener<T>();
         var campos = string.Join(", ", info.Campos.Select(p => p.NombreCampo));
         var parametros = string.Join(", ", info.Campos.Select(p => p.NombreParametro));
         return $"INSERT INTO {info.TablaCompleta} ({campos}) VALUES ({parametros})";
      }

      public static string ArmarUpdate<T>()
          where T : Componente
      {
         var info = ComponenteInfo.Obtener<T>();
         var sets = string.Join(", ", info.Campos.Select(p => $"{p.NombreCampo} = {p.NombreParametro}"));
         var where = string.Join(" AND ", info.Claves.Select(p => $"{p.NombreCampo} = {p.NombreParametro}"));
         return $"UPDATE {info.TablaCompleta} SET {sets} WHERE {where}";
      }

      public static string ArmarDelete<T>(bool fisico, string campoBaja = "Estado", object valorBaja = null)
          where T : Componente
      {
         var info = ComponenteInfo.Obtener<T>();
         var where = string.Join(" AND ", info.Claves.Select(p => $"{p.NombreCampo} = {p.NombreParametro}"));
         if (fisico)
            return $"DELETE FROM {info.TablaCompleta} WHERE {where}";

         valorBaja = valorBaja ?? 0;
         return $"UPDATE {info.TablaCompleta} SET {campoBaja} = {valorBaja} WHERE {where}";
      }

      public static lst.List<FuenteParametro> ArmarParametrosClaves<T>(ComponenteInfo info, object[] valores)
      {
         var parametros = new lst.List<FuenteParametro>();
         for (int i = 0; i < info.Claves.Count && i < valores.Length; i++)
            parametros.Add(new FuenteParametro(info.Claves[i].NombreParametro, valores[i]));
         return parametros;
      }

      public static lst.List<FuenteParametro> ArmarParametrosTodos<T>(ComponenteInfo info, T instancia)
          where T : Componente
      {
         var parametros = new lst.List<FuenteParametro>();
         foreach (var cp in info.Propiedades)
            parametros.Add(new FuenteParametro(cp.NombreParametro, cp.ObtenerValor(instancia)));
         return parametros;
      }
   }

   public class Genialidad
   {
      private readonly FuenteEnlace _enlace;
      private readonly ComponenteValidador _validador;

      public Genialidad(FuenteEnlace enlace, ComponenteValidador validador = null)
      {
         _enlace = enlace;
         _validador = validador ?? new ComponenteValidador { Activo = false };
      }

      // Leer por claves
      public async tsk.Task<T> Leer<T>(ComponenteOpciones opciones = null, params object[] claves)
          where T : Componente, IComponente, new()
      {
         opciones = opciones ?? ComponenteOpciones.Predeterminado;
         var info = ComponenteInfo.Obtener<T>();

         var consulta = new FuenteSqlServer(_enlace.SecuenciaConexion)
         {
            Solicitud = ComponenteConstructor.ArmarSelect<T>(claves),
            Tipo = FuenteConsultaTipo.Personalizado,
            Parametros = ComponenteConstructor.ArmarParametrosClaves<T>(info, claves)
         };

         var respuesta = await consulta.ConsultarAsync(opciones.Cancelacion, opciones.MaxGrillas, opciones.MaxFilas);
         return respuesta.ListaConstruir<T>().FirstOrDefault();
      }

      // Guardar: detecta nuevo vs actualizacion por clave
      public async tsk.Task Guardar<T>(T instancia, ComponenteOpciones opciones = null)
          where T : Componente, IComponente, new()
      {
         opciones = opciones ?? ComponenteOpciones.Predeterminado;
         var info = ComponenteInfo.Obtener<T>();

         var errores = _validador.Validar(instancia);
         if (errores.Count > 0)
            throw new InvalidOperationException(string.Join("; ", errores));

         bool esNuevo = info.Claves.All(cp =>
         {
            var valor = cp.ObtenerValor(instancia);
            return valor == null || valor.Equals(ObtenerDefault(cp.Propiedad.PropertyType));
         });

         var sql = esNuevo
             ? ComponenteConstructor.ArmarInsert<T>()
             : ComponenteConstructor.ArmarUpdate<T>();

         var consulta = new FuenteSqlServer(_enlace.SecuenciaConexion)
         {
            Solicitud = sql,
            Tipo = FuenteConsultaTipo.Personalizado,
            Parametros = ComponenteConstructor.ArmarParametrosTodos<T>(info, instancia)
         };

         await consulta.ConsultarAsync(opciones.Cancelacion);
      }

      // Eliminar: logico por defecto, fisico como opcion
      public async tsk.Task Eliminar<T>(ComponenteOpciones opciones = null, bool fisico = false,
          string campoBaja = "Estado", object valorBaja = null, params object[] claves)
          where T : Componente, IComponente, new()
      {
         opciones = opciones ?? ComponenteOpciones.Predeterminado;
         var info = ComponenteInfo.Obtener<T>();

         var consulta = new FuenteSqlServer(_enlace.SecuenciaConexion)
         {
            Solicitud = ComponenteConstructor.ArmarDelete<T>(fisico, campoBaja, valorBaja),
            Tipo = FuenteConsultaTipo.Personalizado,
            Parametros = ComponenteConstructor.ArmarParametrosClaves<T>(info, claves)
         };

         await consulta.ConsultarAsync(opciones.Cancelacion);
      }

      private object ObtenerDefault(Type tipo)
          => tipo.IsValueType ? Activator.CreateInstance(tipo) : null;
   }


   public class Demo1
   {
      public void Probar()
      {

         // 1. logger primero, antes que todo
         fabrica.Registrar<ILogger>(f => new Logger(new LoggerOpciones
         {
            NivelMinimo = LoggerNivel.Debug,
            Formato = LoggerFormato.Json,
            Carpeta = io.Path.Combine(Application.StartupPath, "logs")
         }), singleton: true);
         var logger = fabrica.Crear<ILogger>();
         logger.Info("Iniciando Amely...");

         ComponenteRegistro.Inicializar(new[]
         {
            rfl.Assembly.GetExecutingAssembly(),
            rfl.Assembly.GetAssembly(typeof(Almacen)), // modulo almacen
            rfl.Assembly.GetAssembly(typeof(Medico)),  // modulo clinico
            rfl.Assembly.GetAssembly(typeof(Factura))  // modulo facturacion
         });

         // configuracion
         var genialidad = new Genialidad(enlace, new ComponenteValidador { Activo = true });

         // leer
         var almacen = await genialidad.Leer<Almacen>(null, "ALM01");

         // leer con opciones
         var opciones = new ComponenteOpciones { MaxFilas = 50 };
         var almacen = await genialidad.Leer<Almacen>(opciones, "ALM01");

         // guardar nuevo (clave vacía)
         var nuevo = new Almacen { Nombre = "Almacen Central", Tipo = "F" };
         await genialidad.Guardar(nuevo);

         // guardar existente (clave con valor)
         almacen.Nombre = "Almacen Norte";
         await genialidad.Guardar(almacen);

         // eliminar logico
         await genialidad.Eliminar<Almacen>(null, false, "Estado", 0, "ALM01");

         // eliminar fisico
         await genialidad.Eliminar<Almacen>(null, true, claves: "ALM01");

         // medico con clave compuesta
         var medico = await genialidad.Leer<Medico>(null, "MED01");
      }
   }
}
