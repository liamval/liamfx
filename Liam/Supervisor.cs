using System;
using gbl = System.Globalization;
using ise = System.Runtime.InteropServices;
using lst = System.Collections.Generic;
using net = System.Net;
using rfl = System.Reflection;
using rsc = System.Resources;
using skt = System.Net.Sockets;

namespace Liam
{
   /// <summary>
   /// Representa el centro de control y supervisión de un programa en tiempo de ejecución.
   /// </summary>
   public class Supervisor
   {
      /// <summary>
      /// Obtiene el indicador de supervisión activado.
      /// </summary>
      public static bool EnLinea { get; private set; }

      /// <summary>
      /// Obtiene el nombre del centro del control.
      /// </summary>
      public static string Nombre { get; private set; }

      /// <summary>
      /// Obtiene la fábrica de componentes.
      /// </summary>
      public static Fabrica Fabrica { get; private set; }

      /// <summary>
      /// Obtiene los derechos de copia del centro del control.
      /// </summary>
      public static string Derechos { get { return LiamInfo?.Derechos ?? string.Empty; } }

      /// <summary>
      /// Obtiene la versión del centro del control.
      /// </summary>
      public static string Version { get { return LiamInfo?.Version ?? string.Empty; } }

      /// <summary>
      /// Obtiene la organización del centro del control.
      /// </summary>
      public static string Organizacion { get { return LiamInfo?.Organizacion ?? string.Empty; } }

      /// <summary>
      /// Obtiene el contexto de la estación que ejecuta el supervisor.
      /// </summary>
      public static EstacionInformacion Estacion { get; private set; }

      /// <summary>
      /// Obtiene el contexto del programa que ejecuta el supervisor.
      /// </summary>
      public static EnsambladoInformacion Ejecutable { get; private set; }

      /// <summary>
      /// Obtiene el diccionario con los gestores de recursos.
      /// </summary>
      public static lst.Dictionary<string, rsc.ResourceManager> RecursosGestor { get; private set; }

      /// <summary>
      /// Obtiene la fecha y hora de inicio de la aplicación.
      /// </summary>
      public static DateTime Inicio { get; private set; }

      /// <summary>
      /// Obtiene el indicador de 64bit de proceso en ejecución.
      /// </summary>
      public static bool Es64Bit { get; internal set; }

      /// <summary>
      /// Lista de los idiomas disponibles para localización.
      /// </summary>
      public static lst.List<Idioma> Idiomas { get; } = new lst.List<Idioma>() {
         new Idioma(codigo: "es-PE", nombreLocal: "Español (Perú)") { EsPrincipal = true },
         new Idioma(codigo: "en-US", nombreLocal: "English (United States)"),
         new Idioma(codigo: "pt-BR", nombreLocal: "Português (Brasil)"),
         new Idioma(codigo: "it-IT", nombreLocal: "Italiano"),
         new Idioma(codigo: "fr-FR", nombreLocal: "Français"),
         new Idioma(codigo: "de-DE", nombreLocal: "Deutsch"),
         new Idioma(codigo: "ru-RU", nombreLocal: "Русский"),
         new Idioma(codigo: "zh-CN", nombreLocal: "中文(中国)")
      };

      /// <summary>
      /// Obtiene el idioma actual del supervisor.
      /// </summary>
      public static Idioma IdiomaActual { get; private set; }

      /// <summary>
      /// Nombre distintivo del ensamblado.
      /// </summary>
      public const string Distintivo = "liam";

      /// <summary>
      /// Evento que se lanza cuando se cambia el idioma del programa.
      /// </summary>
      public static event EventHandler IdiomaCambiado;

      /* Referencia a liam.dll */
      private static EnsambladoInformacion LiamInfo;

      private Supervisor()
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Supervisor"/>.
      /// </summary>
      static Supervisor()
      {
         EnLinea = false;
         Nombre = "Liam";
         Ejecutable = new EnsambladoInformacion() { Nombre = string.Empty, Version = string.Empty };
         Estacion = new EstacionInformacion() { Nombre = string.Empty, PlataformaNombre = string.Empty, PlataformaVersion = string.Empty };
         RecursosGestor = new lst.Dictionary<string, rsc.ResourceManager>(5);
         LiamInfo = null;
      }

      /// <summary>
      /// Inicializa el supervisor del programa.
      /// </summary>
      /// <param name="fabrica">Referencia a la fabrica del programa.</param>
      /// <param name="culturaNombre">Opcional. Indica el nombre del idioma inicial del programa.</param>
      /// <param name="recursoPredefinido">Opcional. Indica el espacio de nombres de referencia a partir de tipo provisto.</param>
      public static void Inicializar(Fabrica fabrica, string culturaNombre = "es-PE", Type recursoPredefinido = null)
      {
         Calentar();

         Fabrica = fabrica;
         Es64Bit = Environment.Is64BitProcess;

         rfl.Assembly liamEnsamblado = rfl.Assembly.GetAssembly(typeof(Componente));
         rfl.Assembly programaEnsamblado = rfl.Assembly.GetEntryAssembly() ?? rfl.Assembly.GetCallingAssembly();

         //RegistrarGestorRecursos(Distintivo, recursoPredefinido ?? typeof(Recursos.Contenedor));

         Estacion = LeerEstacion();
         LiamInfo = LeerEnsamblado(liamEnsamblado);
         Ejecutable = LeerEnsamblado(programaEnsamblado);

         foreach (Idioma i in Idiomas)
         {
            i.Configuracion = new gbl.CultureInfo(i.Codigo);

            if (i.Codigo == "es-PE")
            {
               i.Configuracion.DateTimeFormat.LongDatePattern = "dddd, dd 'de' MMMM 'de' yyyy";
               i.Configuracion.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

               i.Configuracion.DateTimeFormat.LongTimePattern = "hh:mm:ss tt";
               i.Configuracion.DateTimeFormat.ShortTimePattern = "HH:mm";

               i.Configuracion.NumberFormat.PercentPositivePattern = 1; // % pegado
               i.Configuracion.NumberFormat.CurrencyDecimalDigits = 2;
            }
         }

         //Nombre = Utilero.Base64ATexto("U3VzYW4=");
         CambiarIdioma(culturaNombre);

         try
         {
            net.ServicePointManager.ServerCertificateValidationCallback += RedCliente.AceptarCertificados;
         }
         catch (Exception xc)
         {
            Console.WriteLine(xc.ToString());
         }

         Inicio = DateTime.Now;
         EnLinea = true;
      }

      /// <summary>
      /// Recupera una idioma disponible y reasigna la cultura tanto el hilo de ejecución como de la interfaz de usuario del programa.<br/>
      /// NOTA: En aplicaciones Web solo se debe invocar al momento de iniciar la aplicación.
      /// </summary>
      /// <param name="idioma">Nuevo idioma a asignar al programa.</param>
      /// <param name="emisor">Opcional. Generalmente es el objeto que invoca el método. Se recomienda que implemente la interface <see cref="ITraducible"/>.</param>
      /// <remarks>Si el idioma no está disponible, se toma el idioma principal del listado de <see cref="Idiomas"/>.</remarks>
      public static void CambiarIdioma(string idioma, object emisor = null)
      {
         Idioma nuevoIdioma = Idiomas
            .Find(x => x.Codigo == idioma)
            ?? Idiomas.Find(x => x.EsPrincipal)
            ?? throw Utilero.LanzarExcepcion("Liam_CambiarIdiomaError");

         IdiomaActual = nuevoIdioma;
         gbl.CultureInfo.CurrentCulture = IdiomaActual.Configuracion;
         gbl.CultureInfo.CurrentUICulture = IdiomaActual.Configuracion;
         gbl.CultureInfo.DefaultThreadCurrentCulture = IdiomaActual.Configuracion;

         ValorLogico.SiTexto = ObtenerTexto(null, "Liam_Si");
         ValorLogico.NoTexto = ObtenerTexto(null, "Liam_No");

         Utilero.Si = ObtenerTexto(null, "Liam_Si");
         Utilero.No = ObtenerTexto(null, "Liam_No");

         var meses = IdiomaActual.Configuracion.DateTimeFormat.MonthNames;
         Utilero.Meses = new string[] {
            string.Empty, // indice 0
            meses[0], meses[1], meses[2], meses[3],
            meses[4], meses[5], meses[6], meses[7],
            meses[8], meses[9], meses[10], meses[11],
            meses.Length > 12 ? meses[12] : string.Empty // mes 13, calendarios especiales.
         };

         IdiomaCambiado?.Invoke(emisor, EventArgs.Empty);
      }

      /// <summary>
      /// Ejecuta el registrador especificado en un ensamblado.
      /// </summary>
      /// <param name="registrador">Referencia al método registrador.</param>
      public static void RegistrarEnsamblado(Action registrador)
      {
         registrador?.Invoke();
      }

      /// <summary>
      /// Registra un gestor de recursos de un ensamblado.
      /// </summary>
      /// <param name="nombre">Nombre de identificación del gestor.</param>
      /// <param name="recursosFuente">Clase que almacena los recursos de </param>
      /// <remarks>En Visual Basic, acceder a través del espacio de nombres My causa errores, usar a cambio el método <see cref="RegistrarGestorRecursos(string, string, Assembly)"/>.</remarks>
      public static void RegistrarGestorRecursos(string nombre, Type recursosFuente)
      {
         if (!RecursosGestor.ContainsKey(nombre))
            RecursosGestor.Add(nombre, new rsc.ResourceManager(recursosFuente));
         else
            throw Utilero.LanzarExcepcion("Liam_GestorRecursosDuplicado");
      }

      /// <summary>
      /// Registra un gestor de recursos de un ensamblado.
      /// </summary>
      /// <param name="nombre">Nombre de identificación del gestor.</param>
      /// <param name="recursoNombreCompleto">Nombre completo calificado de la clase del gestor de recursos.</param>
      /// <param name="ensamblado">Ensamblado donde se enuentra el gestor de recursos.</param>
      public static void RegistrarGestorRecursos(string nombre, string recursoNombreCompleto, rfl.Assembly ensamblado = null)
      {
         if (!RecursosGestor.ContainsKey(nombre))
            RecursosGestor.Add(nombre, new rsc.ResourceManager(recursoNombreCompleto, ensamblado ?? rfl.Assembly.GetExecutingAssembly()));
         else
            throw Utilero.LanzarExcepcion("Liam_GestorRecursosDuplicado");
      }

      /// <summary>
      /// Devuelve el recurso de texto <paramref name="recursoNombre"/> desde el contenedor <paramref name="contenedorNombre"/>.
      /// </summary>
      /// <param name="contenedorNombre">Nombre de identificación del gestor.</param>
      /// <param name="recursoNombre">Nombre de del recurso.</param>
      /// <param name="idiomaCodigo">Opcional. Código de idioma.</param>
      /// <param name="valores">Opcional. Lista de valores para reemplazar en el texto.</param>
      /// <returns>Recurso de texto <paramref name="recursoNombre"/> desde el contenedor <paramref name="contenedorNombre"/>.</returns>
      public static string ObtenerTexto(string contenedorNombre, string recursoNombre, string idiomaCodigo = null, object[] valores = null)
      {
         if (contenedorNombre is null)
            contenedorNombre = Distintivo;

         RecursosGestor.TryGetValue(contenedorNombre, out rsc.ResourceManager contenedorRecursos);
         if (contenedorRecursos is null)
            return $"{contenedorNombre}:{recursoNombre.Replace("Liam_", "")}";

         Idioma idioma = idiomaCodigo is null ? IdiomaActual : Idiomas.Find(x => x.Codigo.ToLower() == idiomaCodigo.ToLower());

         string texto =
            contenedorRecursos.GetString(recursoNombre, idioma?.Configuracion)
            ?? $"#{recursoNombre.Replace("Liam_", "")}#";

         if (valores != null)
            texto = string.Format(texto, valores);

         return texto;
      }

      /// <summary>
      /// Devuelve el recurso de imagen <paramref name="recursoNombre"/> desde el contenedor <paramref name="contenedorNombre"/>.
      /// </summary>
      /// <param name="contenedorNombre">Nombre de identificación del gestor.</param>
      /// <param name="recursoNombre">Nombre de del recurso.</param>
      /// <param name="idiomaCodigo">Opcional. Código de cultura.</param>
      /// <returns>Recurso de imagen <paramref name="recursoNombre"/> desde el contenedor <paramref name="contenedorNombre"/>.</returns>
      public static object ObtenerContenido(string contenedorNombre, string recursoNombre, string idiomaCodigo = null)
      {
         if (contenedorNombre is null)
            contenedorNombre = Distintivo;

         RecursosGestor.TryGetValue(contenedorNombre, out rsc.ResourceManager contenedorRecursos);
         if (contenedorRecursos is null)
            return null;

         Idioma idioma = idiomaCodigo is null ? IdiomaActual : Idiomas.Find(x => x.Codigo.ToLower() == idiomaCodigo.ToLower());

         object contenido = contenedorRecursos.GetObject(recursoNombre, idioma?.Configuracion);

         return contenido;
      }

      /// <summary>
      /// Lee y devuelve las propiedades del contexto de un ensamblado.
      /// </summary>
      /// <param name="assembly">
      /// Ensamblado seleccionado (opcional).
      /// Si es null (Nothing en Visual Basic) se obtiene un ensamblado invocando el método <see cref="Assembly.GetEntryAssembly"/>.
      /// Si continúa siendo null (Nothing en Visual Basic), se invoca el método <see cref="Assembly.GetExecutingAssembly"/>.
      /// </param>
      /// <returns>Propiedades del contexto de un ensamblado.</returns>
      public static EnsambladoInformacion LeerEnsamblado(rfl.Assembly assembly)
      {
         try
         {
            EnsambladoInformacion ensamblado = new EnsambladoInformacion();

            rfl.Assembly entry = assembly ?? rfl.Assembly.GetEntryAssembly() ?? rfl.Assembly.GetExecutingAssembly();
            object[] attrTitle = entry.GetCustomAttributes(typeof(rfl.AssemblyTitleAttribute), false);
            object[] attrProduct = entry.GetCustomAttributes(typeof(rfl.AssemblyProductAttribute), false);
            object[] attrOrg = entry.GetCustomAttributes(typeof(rfl.AssemblyCompanyAttribute), false);
            object[] attrCopy = entry.GetCustomAttributes(typeof(rfl.AssemblyCopyrightAttribute), false);
            object[] attrDescr = entry.GetCustomAttributes(typeof(rfl.AssemblyDescriptionAttribute), false);
            rfl.Module[] mdls = entry.GetModules();
            Version ver = entry.GetName().Version;

            ensamblado.IdiomaCodigo = entry.GetName().CultureName;
            ensamblado.Version = $"{ver.Major:0}.{ver.Minor:0}.{ver.Build:0}.{ver.Revision:0}";
            ensamblado.Nombre = attrTitle?.Length > 0 ? (attrTitle[0] as rfl.AssemblyTitleAttribute).Title : string.Empty;
            ensamblado.Producto = attrProduct?.Length > 0 ? (attrProduct[0] as rfl.AssemblyProductAttribute)?.Product : string.Empty;
            ensamblado.Organizacion = attrOrg?.Length > 0 ? (attrOrg[0] as rfl.AssemblyCompanyAttribute)?.Company : string.Empty;
            ensamblado.Derechos = attrCopy?.Length > 0 ? (attrCopy[0] as rfl.AssemblyCopyrightAttribute)?.Copyright : string.Empty;
            ensamblado.Descripcion = attrDescr?.Length > 0 ? (attrDescr[0] as rfl.AssemblyDescriptionAttribute)?.Description : string.Empty;
            ensamblado.ModuloNombre = mdls?.Length > 0 ? mdls[0].Name : string.Empty;
            ensamblado.ModuloId = mdls?.Length > 0 ? mdls[0].ModuleVersionId.ToString("B") : string.Empty;

            return ensamblado;
         }
         catch (Exception xc)
         {
            throw Utilero.LanzarExcepcion("Liam_InicializacionError", excepcionInterna: xc);
         }
      }

      private static void Calentar()
      {
         // Inicialización de estructuras SEH (para mejorar el rendimiento).
         try
         {
            throw new LiamException(Distintivo);
         }
         catch (LiamException)
         {
            // en blanco intencionalmente.
         }
      }

      /// <summary>
      /// Devuelve las propiedades del contexto de la estación.
      /// </summary>
      /// <returns>Propiedades del contexto de la estación.</returns>
      public static EstacionInformacion LeerEstacion()
      {
         try
         {
            string estacionLocal = net.Dns.GetHostName();
            EstacionInformacion estacion = new EstacionInformacion() { };
            net.IPAddress[] addresses = net.Dns.GetHostEntry(estacionLocal).AddressList;
            net.IPAddress selAddr = null;
            net.IPAddress selAddrv6 = null;
            if (addresses != null && addresses.Length > 0)
            {
               foreach (var addr in addresses)
               {
                  if (addr.AddressFamily == skt.AddressFamily.InterNetwork && selAddr is null)
                  {
                     selAddr = addr;
                  }
                  if (addr.AddressFamily == skt.AddressFamily.InterNetworkV6 && selAddrv6 is null)
                  {
                     selAddrv6 = addr;
                  }
                  if (selAddr != null && selAddrv6 != null)
                     break;
               }
            }
            estacion.Nombre = estacionLocal;
            estacion.MotorVersion = Environment.Version.ToString();
            estacion.MotorDescripcion = ise.RuntimeInformation.FrameworkDescription;

            estacion.DireccionIPv4 = selAddr?.ToString() ?? ObtenerTexto(null, "Liam_Null", "en-US");
            estacion.DireccionIPv6 = selAddrv6?.ToString() ?? ObtenerTexto(null, "Liam_Null", "en-US");

            estacion.PlataformaUsuario = Environment.UserName;
            estacion.PlataformaNombre = Environment.OSVersion.Platform.ToString();
            estacion.PlataformaVersion = Environment.OSVersion.Version.ToString();
            estacion.PlataformaEs64Bit = Environment.Is64BitOperatingSystem;

            return estacion;
         }
         catch (Exception xc)
         {
            throw Utilero.LanzarExcepcion("Liam_InicializacionError", excepcionInterna: xc);
         }
      }
   }
}
