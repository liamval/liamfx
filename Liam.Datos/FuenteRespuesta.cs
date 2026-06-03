using System;
using lst = System.Collections.Generic;

namespace Liam.Datos
{
   /// <summary>
   /// Representa la respuesta de una consulta a una fuente de datos, conteniendo una o más tablas de resultados y métodos para construir componentes a partir de esos resultados.
   /// </summary>
   public class FuenteRespuesta
   {
      /// <summary>
      /// Obtiene las tablas de resultados devueltas por la consulta a la fuente de datos.
      /// </summary>
      public lst.List<FuenteTabla> Tablas { get; internal set; }

      /// <summary>
      /// Obtiene un valor de retorno cuando se ejecuta un método en la fuente de datos y que no forma parte de las tablas de resultados.
      /// </summary>
      public object Retorno { get; internal set; }

      /// <summary>
      /// Obtiene un arreglo de objetos representando los valores de la primera fila de la primera tabla de resultados, o null si no hay tablas o filas disponibles.
      /// </summary>
      public object[] Lista0
      {
         get
         {
            if (this.Tablas.Count > 0)
               return Tablas[0].Lista0;

            return null;
         }
      }

      /// <summary>
      /// Obtiene un diccionario de clave-valor representando los datos de la primera fila de la primera tabla de resultados, o null si no hay tablas o filas disponibles.
      /// </summary>
      public lst.Dictionary<string, object> Fila0
      {
         get
         {
            if (this.Tablas.Count > 0)
               return Tablas[0].Fila0;

            return null;
         }
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteRespuesta"/>.
      /// </summary>
      public FuenteRespuesta()
      {
         this.Inicializar();
      }

      private void Inicializar()
      {
         this.Tablas = new lst.List<FuenteTabla>();
      }

      /// <summary>
      /// Devuelve un componente del tipo especificado construido a partir de los datos de la fila indicada en la tabla indicada, o un componente con datos vacíos si no hay tablas o filas disponibles. El método también acepta una acción opcional para configurar el componente después de su construcción.
      /// </summary>
      /// <typeparam name="TComponente">Tipo de dato del componente a construir.</typeparam>
      /// <param name="tablaIndice">Índice de la tabla.</param>
      /// <param name="filaIndice">Índice de la fila dentro de la tabla.</param>
      /// <param name="configurarComponente">(Opcional) Acción para post-procesar el componente.</param>
      /// <returns>Componente del tipo especificado construido a partir de los datos de la fila indicada en la tabla indicada.</returns>
      public TComponente Construir<TComponente>(int tablaIndice = 0, int filaIndice = 0, Action<TComponente> configurarComponente = null)
         where TComponente : Componente, IComponente, new()
      {
         FuenteFila fila = null;
         if (this.Tablas.Count > tablaIndice && this.Tablas[tablaIndice].Filas.Count > filaIndice)
            fila = this.Tablas[tablaIndice].Filas[filaIndice];

         var registro = ComponenteInstanciar<TComponente>(fila);
         configurarComponente?.Invoke(registro);

         return registro;
      }

      /// <summary>
      /// Devuelve una lista de componentes del tipo especificado construidos a partir de los datos de la tabla indicada, o una lista vacía si no hay tablas o filas disponibles. El método también acepta una acción opcional para configurar cada componente después de su construcción.
      /// </summary>
      /// <typeparam name="TComponente">Tipo de dato del componente a construir.</typeparam>
      /// <param name="tablaIndice">Índice de la tabla.</param>
      /// <param name="configurarComponente">(Opcional) Acción para post-procesar el componente.</param>
      /// <returns>Lsta de componentes del tipo especificado construidos a partir de los datos de la tabla indicada.</returns>
      public lst.List<TComponente> ListaConstruir<TComponente>(int tablaIndice = 0, Action<TComponente> configurarComponente = null)
         where TComponente : Componente, IComponente, new()
      {
         var lista = new lst.List<TComponente>();

         if (this.Tablas.Count < tablaIndice + 1)
            return lista;

         var datos = this.Tablas[tablaIndice].Filas;

         if (datos.Count == 0)
            return lista;

         foreach (var fila in datos)
         {
            var registro = ComponenteInstanciar<TComponente>(fila);
            configurarComponente?.Invoke(registro);
            lista.Add(registro);
         }

         return lista;
      }

      /// <summary>
      /// Devuelve un diccionario de componentes del tipo especificado construidos a partir de los datos de la tabla indicada, utilizando una función para obtener la clave de cada componente. El método también acepta una acción opcional para configurar cada componente después de su construcción. Si no hay tablas o filas disponibles, se devuelve un diccionario vacío.
      /// </summary>
      /// <typeparam name="TClave">Tipo de dato de la clave del diccionario.</typeparam>
      /// <typeparam name="TComponente">Tipo de dato del componente a construir.</typeparam>
      /// <param name="obtenerClave">Función para obtener la clave del diccionario.</param>
      /// <param name="tablaIndice">Índice de la tabla.</param>
      /// <param name="configurarComponente">(Opcional) Acción para post-procesar el componente.</param>
      /// <returns>Diccionario de componentes del tipo especificado construidos a partir de los datos de la tabla indicada.</returns>
      /// <exception cref="ArgumentNullException">Si no se proporciona la función para la clave del diccionario.</exception>
      public lst.Dictionary<TClave, TComponente> DiccionarioConstruir<TClave, TComponente>(Func<TComponente, TClave> obtenerClave, int tablaIndice = 0, Action<TComponente> configurarComponente = null)
          where TComponente : Componente, IComponente, new()
      {
         if (obtenerClave is null)
            throw new ArgumentNullException(nameof(obtenerClave));

         var diccionario = new lst.Dictionary<TClave, TComponente>();

         if (this.Tablas.Count < tablaIndice + 1)
            return diccionario;

         var datos = this.Tablas[tablaIndice].Filas;

         if (datos.Count == 0)
            return diccionario;

         foreach (var fila in datos)
         {
            var registro = ComponenteInstanciar<TComponente>(fila);
            configurarComponente?.Invoke(registro);
            var clave = obtenerClave(registro);
            diccionario[clave] = registro;
         }

         return diccionario;
      }

      /// <summary>
      /// Devuelve un objeto de tipo <see cref="Busqueda"/> que contiene una lista de componentes del tipo especificado construidos a partir de los datos de la tabla indicada, o una lista vacía si no hay tablas o filas disponibles. El método también acepta una acción opcional para configurar cada componente después de su construcción.
      /// </summary>
      /// <typeparam name="TComponente">Tipo de dato del componente a construir.</typeparam>
      /// <param name="tablaIndice">Índice de la tabla.</param>
      /// <param name="configurarComponente">(Opcional) Acción para post-procesar el componente.</param>
      /// <returns>Objeto de tipo <see cref="Busqueda"/> que contiene una lista de componentes del tipo especificado construidos a partir de los datos de la tabla indicada.</returns>
      /// <remarks>
      /// Este método envuelve a <see cref="ListaConstruir{TComponente}(int, Action{TComponente})"/> para su uso en escenarios donde se espera un resultado de búsqueda, proporcionando una estructura de datos genérica para ese propósito.
      /// </remarks>
      public Busqueda PaginaConstruir<TComponente>(int tablaIndice = 0, Action<TComponente> configurarComponente = null)
         where TComponente : Componente, IComponente, new()
      {
         var busqueda = new Busqueda();
         var lista = this.ListaConstruir(tablaIndice, configurarComponente);
         busqueda.Resultados.AddRange(lista);

         return busqueda;
      }

      /// <summary>
      /// Crea una nueva instancia de un objeto basado en <see cref="Componente"/>.
      /// </summary>
      /// <typeparam name="T">Clase derivada de <see cref="Componente"/>.</typeparam>
      /// <param name="datos">Datos para asignar a las propiedades del objeto.</param>
      public T ComponenteInstanciar<T>(object datos) where T : Componente, new()
      {
         var ret = new T();

         if (datos is FuenteFila fila)
            FilaMapear(fila, ref ret);

         return ret;
      }

      protected internal void FilaMapear<T>(FuenteFila datos, ref T ret) where T : Componente, IComponente, new()
      {
         Type inst = ret.GetType();
         var infoCmpnt = ComponenteRegistro.Obtener(inst);
         object valor;
         object valorPredef;
         int datoIndice;
         bool usarPredef;
         bool tienePredef;

         foreach (var propSel in infoCmpnt.Propiedades)
         {
            valor = null;
            valorPredef = null;
            usarPredef = false;
            tienePredef = false;

            datoIndice = datos.ObtenerIndice(propSel.CampoNombre);

            if (datoIndice >= 0)
            {
               valor = datos[datoIndice];
               MostrarSalida($" dc:= ", false);
            }
            else
            {
               MostrarSalida($" (no):= ", false);
            }

            if (propSel.ValorPredefinido != null)
            {
               tienePredef = true;
               valorPredef = propSel.ValorPredefinido.Valor;
            }

            if (valor is DBNull || valor is null)
            {
               valor = null;
               usarPredef = true;
            }
            else
            {
               if (propSel.Condiciones != null)
                  EvaluarCondiciones(propSel.Condiciones.Condiciones, ref valor, ref usarPredef);
            }

            if (usarPredef && tienePredef)
            {
               valor = valorPredef;
               MostrarSalida($"<df>", false);
            }

            MostrarSalida($"[{valor ?? "#null#"}] {valor?.GetType().Name ?? "-"}");

            if (!(valor is null))
            {
               propSel.ValorAsignar(ret, valor);
            }
         }
      }

      private static void MostrarSalida(string salida, bool finLinea = true)
      {
#if DEBUG1
         if (finLinea)
            System.Diagnostics.Debug.WriteLine(salida);
         else
            System.Diagnostics.Debug.Write(salida);
#endif
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="condiciones"></param>
      /// <param name="valor"></param>
      /// <param name="usarPredef"></param>
      protected internal void EvaluarCondiciones(DatoCondicion condiciones, ref object valor, ref bool usarPredef)
      {
         bool tieneConvertirANumero = false;

         if ((condiciones & DatoCondicion.ComoNumero32) == DatoCondicion.ComoNumero32)
         {
            valor = Convert.ChangeType(valor, typeof(int));
            tieneConvertirANumero = true;
         }
         else if ((condiciones & DatoCondicion.ComoNumero64) == DatoCondicion.ComoNumero64)
         {
            valor = Convert.ChangeType(valor, typeof(long));
            tieneConvertirANumero = true;
         }
         else if ((condiciones & DatoCondicion.ComoNumeroDec) == DatoCondicion.ComoNumeroDec)
         {
            valor = Convert.ChangeType(valor, typeof(decimal));
            tieneConvertirANumero = true;
         }
         if (tieneConvertirANumero)
         {
            if ((condiciones & DatoCondicion.NumeroCeroComoNulo) == DatoCondicion.NumeroCeroComoNulo)
            {
               if ((decimal)valor == 0m)
               {
                  valor = null;
                  usarPredef = true;
               }
            }
         }

         if ((condiciones & DatoCondicion.ComoTexto) == DatoCondicion.ComoTexto)
         {
            valor = Convert.ChangeType(valor, typeof(string));
         }

         if (valor is string auxValor)
         {
            // primero quitar espacios
            if ((condiciones & DatoCondicion.QuitarRelleno) == DatoCondicion.QuitarRelleno)
            {
               if (auxValor != null)
               {
                  auxValor = auxValor.Trim();
               }
            }
            // pasar a null si es cadena vacía.
            if ((condiciones & DatoCondicion.TextoVacioComoNulo) == DatoCondicion.TextoVacioComoNulo)
            {
               if (auxValor == string.Empty)
               {
                  auxValor = null;
                  usarPredef = true;
               }
            }
            // las demas operaciones (si hay cadena disponible)
            if (auxValor != null)
            {
               if ((condiciones & DatoCondicion.TransformarMinuscula) == DatoCondicion.TransformarMinuscula)
               {
                  auxValor = auxValor.ToLower();
               }
               if ((condiciones & DatoCondicion.TransformarMayuscula) == DatoCondicion.TransformarMayuscula)
               {
                  auxValor = auxValor.ToUpper();
               }
            }
            valor = auxValor;
         }
      }
   }
}
