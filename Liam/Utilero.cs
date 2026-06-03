using System;
using System.Linq;
using cfg = System.Configuration;
using crp = System.Security.Cryptography;
using dgx = System.Diagnostics;
using dtc = System.Data;
using lst = System.Collections.Generic;
using rgx = System.Text.RegularExpressions;
using sql = System.Data.SqlClient;
using stm = System.IO;
using thr = System.Threading;
using tsk = System.Threading.Tasks;
using txt = System.Text;

namespace Liam
{
   /// <summary>
   /// Representa el asistente del supervisor.
   /// </summary>
   public class Utilero
   {
      /// <summary>
      /// Representa el decimal 100.
      /// </summary>
      public const decimal Cien = 100.00m;

      /// <summary>
      /// Representa el decimal 0.
      /// </summary>
      public const decimal Cero = 0.00m;

      /// <summary>
      /// Representa el decimal 1.
      /// </summary>
      public const decimal Uno = 1.00m;

      /// <summary>
      /// Representa el decimal -1.
      /// </summary>
      public const decimal UnoNegativo = -1.00m;

      /// <summary>
      /// Obtiene el mejor año = 1984.
      /// </summary>
      public const int NumeroBase = 1984;

      /// <summary>
      /// Representa el texto "Si".
      /// </summary>
      public static string Si { get; internal set; }

      /// <summary>
      /// Representa el texto "No".
      /// </summary>
      public static string No { get; internal set; }

      /// <summary>
      /// Contiene la lista de los meses con su índice corregido (Ejemplo: 2 es febrero).
      /// </summary>
      public static string[] Meses { get; internal set; }

      /// <summary>
      /// Obtiene el descriptor de cabecera para el entorno.
      /// </summary>
      public static string XEntorno => "X-LIAM-Entorno";

      /// <summary>
      /// Obtiene el descriptor de cabecera para el núcleo.
      /// </summary>
      public static string XNucleo => "X-LIAM-Nucleo";

      /// <summary>
      /// Obtiene el descriptor de cabecera para la versión.
      /// </summary>
      public static string XVersion => "X-LIAM-Version";

      /// <summary>
      /// Devuelve el valor obtenido desde el archivo de configuración de un aplicación.
      /// </summary>
      /// <param name="nombre">Nombre de la configuración</param>
      /// <param name="predefinido">Valor predefinido en caso no exista la configuración.</param>
      /// <returns>Valor obtenido desde el archivo de configuración de un aplicación.</returns>
      public static string LeerConfiguracion(string nombre, string predefinido = null)
      {
         var valor = cfg.ConfigurationManager.AppSettings[nombre] ?? predefinido;

         return valor;
      }

      /// <summary>
      /// Devuelve la cadena de conexión obtenida desde el archivo de configuración de un aplicación.
      /// </summary>
      /// <param name="nombre">Nombre de la configuración</param>
      /// <param name="predefinido">Valor predefinido en caso no exista la configuración.</param>
      /// <returns>Cadena de conexión obtenida desde el archivo de configuración de un aplicación.</returns>
      public static string LeerConexionCadena(string nombre)
      {
         var valor = cfg.ConfigurationManager.ConnectionStrings[nombre].ConnectionString;

         return valor;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="texto"></param>
      /// <param name="predefinido"></param>
      /// <returns></returns>
      public static int AsegurarNumero32(string texto, int predefinido = 0)
      {
         var ret = int.TryParse(texto, out int val);
         if (!ret)
            val = predefinido;

         return val;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="texto"></param>
      /// <param name="predefinido"></param>
      /// <returns></returns>
      public static long AsegurarNumero64(string texto, long predefinido = 0L)
      {
         var ret = long.TryParse(texto, out long val);
         if (!ret)
            val = predefinido;

         return val;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="objeto"></param>
      /// <param name="predefinido"></param>
      /// <returns></returns>
      public static long AsegurarNumero64(object objeto, long predefinido = 0L)
      {
         return AsegurarNumero64(objeto?.ToString(), predefinido);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="texto"></param>
      /// <param name="predefinido"></param>
      /// <returns></returns>
      public static decimal AsegurarNumeroDec(string texto, decimal predefinido = 0m)
      {
         var ret = decimal.TryParse(texto, out decimal val);
         if (!ret)
            val = predefinido;

         return val;
      }

      /// <summary>
      /// Devuelve un número decimal con redondeado convencional.
      /// </summary>
      /// <param name="numero">Número a redondear.</param>
      /// <param name="precision">Cantidad de decimales a considerar en el redondeo.</param>
      /// <returns>Número decimal con redondeado convencional.</returns>
      public static decimal Redondear(decimal numero, int precision = 2)
      {
         return Math.Round(numero, precision, MidpointRounding.AwayFromZero);
      }

      /// <summary>
      /// Devuelve un vector con las secciones de una cadena de texto cortados según separadores.
      /// </summary>
      /// <param name="codigo"></param>
      /// <param name="secciones"></param>
      /// <param name="soloNumeros">Indica si cada sección debe ser solo numérica, en caso constrario se descarta todo.</param>
      /// <returns></returns>
      public static string[] InterpretarCodigo(string codigo, int secciones, bool soloNumeros = false)
      {
         if (codigo is null)
            return null;

         var cortes = new char[] { '-', '.', '/', ' ', (char)8 };

         string[] vals = codigo.Split(cortes, StringSplitOptions.RemoveEmptyEntries);

         bool ok = false;
         if (vals.GetLength(0) >= secciones)
         {
            ok = true;
            if (soloNumeros)
            {
               for (int i = 0; i < secciones; i++)
                  ok &= int.TryParse(vals[i], out _);
            }
         }

         return ok ? vals : null;
      }

      /// <summary>
      /// Devuelve una cadena de texto en formato base64 a partir de una cadena de texto inicial.
      /// </summary>
      /// <param name="texto">Cadena de texto inicial.</param>
      /// <returns>Cadena de texto en formato base64 a partir de una cadena de texto inicial.</returns>
      public static string TextoABase64(string texto)
      {
         byte[] bytes = txt.Encoding.UTF8.GetBytes(texto);
         var ret = Convert.ToBase64String(bytes);

         return ret;
      }

      /// <summary>
      /// Devuelve la cadena de texto inicial a partir de una cadena de texto en formato base64.
      /// </summary>
      /// <param name="textoBase64">Cadena de texto en formato base64.</param>
      /// <returns>Cadena de texto inicial a partir de una cadena de texto en formato base64.</returns>
      public static string Base64ATexto(string textoBase64)
      {
         byte[] bytes = Convert.FromBase64String(textoBase64);
         var ret = txt.Encoding.UTF8.GetString(bytes);

         return ret;
      }

      /// <summary>
      /// Devuelve un numero extraído desde una cadena de texto.
      /// </summary>
      /// <param name="texto">Texto donde se hará la búsqueda.</param>
      /// <param name="inicio">Indice de inicio de la búsqueda.</param>
      /// <returns>Numero extraído desde una cadena de texto.</returns>
      public static decimal ExtraerNumero(string texto, int inicio = 0)
      {
         if (string.IsNullOrWhiteSpace(texto))
            return Cero;

         txt.StringBuilder cpg = new txt.StringBuilder();
         cpg.Append('0');
         bool decSep = false;
         for (int i = inicio; i < texto.Length; i++)
         {
            if (char.IsDigit(texto[i]))
               cpg.Append(texto[i]);
            else
            {
               if (texto[i] == '.' || texto[i] == ',')
               {
                  if (!decSep)
                  {
                     cpg.Append('.');
                     decSep = true;
                  }
                  else
                  {
                     break;
                  }
               }
               else //if (texto[i] == ' ' || texto[i] == '%')
                  break;
            }
         }
         if (decSep)
            cpg.Append('0');

         if (!decimal.TryParse(cpg.ToString(), out decimal ret))
            ret = Cero;

         return ret;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="TEntrada"></typeparam>
      /// <typeparam name="TResultado"></typeparam>
      /// <param name="lista"></param>
      /// <param name="transformador"></param>
      /// <returns></returns>
      public static lst.List<TResultado> Transformar<TEntrada, TResultado>(lst.List<TEntrada> lista, Func<TEntrada, TResultado> transformador)
      {
         lst.List<TResultado> resultado = new lst.List<TResultado>();

         foreach (var elemento in lista)
         {
            resultado.Add(transformador(elemento));
         }

         return resultado;
      }

      /// <summary>
      /// Devuelve la fecha y hora actual del sistema.
      /// </summary>
      /// <returns>Fecha y hora actual del sistema.</returns>
      public static DateTime FechaActual()
      {
         return DateTime.Now;
      }

      /// <summary>
      /// Devuelve un valor para filtro (o busquedas) con una expresión comodín.
      /// </summary>
      /// <param name="filtro">Valor de búsqueda (puede contener ya el comodín).</param>
      /// <param name="comodin">expresión comodín a adjuntar al final (si es que no lo tiene).</param>
      /// <returns>Valor para filtro (o busquedas) con una expresión comodín.</returns>
      /// <remarks>Esto permite que sea posible incluir el comodín desde la entrada de usuario.</remarks>
      public static string FiltroComodin(string filtro, string comodin = "%")
      {
         if (string.IsNullOrWhiteSpace(filtro))
            return comodin;

         if (filtro.Contains(comodin))
            return filtro;

         return filtro + comodin;
      }

      /// <summary>
      /// Devuelve un indicador verdadero si la dirección cumple con el formato <a href="https://datatracker.ietf.org/doc/html/rfc5322">RFC 5322</a> o falso en caso contrario.
      /// NOTA: Hay una validación previa de longitudes, el nombre de usuario no debe exceder de 64 caracteres y la dirección completa los 255 caracteres.
      /// </summary>
      /// <param name="direccion">Dirección de correo electrónico.</param>
      /// <returns>Un indicador verdadero si la dirección cumple con el formato RFC 5322 o falso en caso contrario.</returns>
      public static ValorLogico ValidarCorreo(string direccion)
      {
         string regexCorreo =
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@" +
            @"[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?" +
            @"(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

         if (direccion.IndexOf('@') > 64 || direccion.Length > 255)
            return ValorLogico.No;

         //string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
         bool ret = rgx.Regex.IsMatch(direccion, regexCorreo);
         //ret = ret && rgx.Regex.Replace(direccion, expresion, string.Empty).Length == 0;

         return ret;
      }

      /// <summary>
      /// Devuelve un par de valores "A" y "B" a partir de la cadena de texto "A (B)".
      /// </summary>
      /// <param name="texto">cadena de texto con el formato "A (B)"</param>
      /// <returns></returns>
      public static string[] SepararNombreyDescripcion(string texto)
      {
         var match = rgx.Regex.Match(texto, @"^\s*(.*?)\s*(?:\(\s*(.*?)\s*\))?\s*$");

         string parte1 = match.Groups[1].Value;
         string parte2 = match.Groups[2].Success ? match.Groups[2].Value : parte1;

         return new string[] { parte1, parte2 };
      }

      /// <summary>
      /// Invoca un proceso externo a criterio del sistema operativo para mostrar una Url.
      /// </summary>
      /// <param name="url">Dirección para mostrar.</param>
      public static void LanzarNavegador(string url)
      {
         dgx.Process.Start(new dgx.ProcessStartInfo(url) { UseShellExecute = true });
      }

      /// <summary>
      /// Invoca al sistema operativo la ejecución de un proceso.
      /// </summary>
      /// <param name="nombre">Nombre (y ruta) de la aplicación.</param>
      /// <param name="argumentos">Argumentos para la invocación de la aplicación.</param>
      /// <param name="accion">Tipo de descriptor para el sistema operativo.</param>
      /// <param name="usarSistema">Indicador de uso de entorno del sistema operativo.</param>
      /// <returns>Instancia del proceso en ejecución.</returns>
      public static dgx.Process LanzarAplicacion(string nombre, string argumentos, string accion = "runas", bool usarSistema = true)
      {
         var iniciar = new dgx.ProcessStartInfo
         {
            FileName = nombre,
            Arguments = argumentos,
            Verb = accion,
            UseShellExecute = usarSistema
         };
         var proceso = dgx.Process.Start(iniciar);

         return proceso;
      }

      /// <summary>
      /// Devuelve una cadena de texto con una huella digital de un contenido en bytes.
      /// </summary>
      /// <param name="contenido">contenido en bytes.</param>
      /// <returns>Cadena de texto con una huella digital de un contenido en bytes.</returns>
      public static string ObtenerHuella(byte[] contenido)
      {
         using (var sha = crp.SHA256.Create())
         {
            byte[] hashBytes = sha.ComputeHash(contenido);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
         }
      }

      /// <summary>
      /// Leer el byte[] de un archivo.
      /// </summary>
      /// <param name="destino">Ruta y nombre del archivo a leer</param>
      /// <returns>byte[] de un archivo.</returns>
      public static async tsk.Task<byte[]> ArchivoLeer(string destino)
      {
         byte[] contenido = null;

         try
         {
            using (stm.FileStream fs = new stm.FileStream(destino, stm.FileMode.OpenOrCreate, stm.FileAccess.Read, stm.FileShare.Read, 4096, useAsync: true))
            {
               contenido = new byte[fs.Length];
               var total = await fs.ReadAsync(contenido, 0, contenido.Length);
            }
         }
         catch (Exception)
         {
            //Supervisor.RegistrarEntrada(xc);
         }

         return contenido;
      }

      /// <summary>
      /// Devuelve el indicador de completado al guardar el byte[] de un archivo.
      /// </summary>
      /// <param name="destino">Ruta y nombre del archivo a guardar</param>
      /// <param name="contenido">byte[] a guardar</param>
      /// <param name="reEscribir">Indicador si debe reescribirse.</param>
      /// <returns>Indicador de completado al guardar el byte[] de un archivo.</returns>
      public static async tsk.Task<bool> ArchivoGuardar(string destino, byte[] contenido, bool reEscribir)
      {
         var retval = false;
         try
         {
            if (stm.File.Exists(destino))
            {
               if (reEscribir)
                  stm.File.Delete(destino);
               else
                  return retval;
            }

            stm.Directory.CreateDirectory(stm.Path.GetDirectoryName(destino));

            using (stm.FileStream fs = new stm.FileStream(destino, stm.FileMode.Create, stm.FileAccess.Write, stm.FileShare.None, 4096, useAsync: true))
            {
               await fs.WriteAsync(contenido, 0, contenido.Length);
               retval = true;
            }
         }
         catch (Exception)
         {
            //Supervisor.RegistrarEntrada(xc);
         }

         return retval;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="nombre"></param>
      /// <returns></returns>
      public static string ObtenerTipoGenerico(string nombre)
      {
         var tipo = "Documentos";
         var fi = new stm.FileInfo(nombre);
         switch (fi.Extension)
         {
            case ".mp3":
            case ".m4a":
            case ".wav":
            case ".ogg":
            case ".wma":
               tipo = "Audios"; break;
            case ".dll":
            case ".exe":
            case ".bin":
            case ".configuration":
            case ".config":
            case ".dbg":
            case ".reg":
            case ".img":
               tipo = "Ensamblados"; break;
            case ".msj":
            case ".eml":
               tipo = "Mensajes"; break;
            case ".mpg":
            case ".mp4":
            case ".avi":
            case ".mpeg":
            case ".wmv":
               tipo = "Videos"; break;
            case ".txt":
               tipo = "Notas"; break;
            case ".jpg":
            case ".jpeg":
            case ".gif":
            case ".png":
            case ".tiff":
            case ".webp":
            case ".bmp":
               tipo = "Imagenes"; break;
         }

         return tipo;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="fileBytes"></param>
      /// <returns></returns>
      public static string ObtenerMimeTipo(byte[] fileBytes)
      {
         if (fileBytes is null || fileBytes.Length < 4)
            return "application/octet-stream";

         int length = fileBytes.Length >= 8 ? 8 : fileBytes.Length;
         byte[] headerBytes = new byte[length];
         Array.Copy(fileBytes, headerBytes, length);

         string hex = BitConverter.ToString(headerBytes).Replace("-", " ");

         if (hex.StartsWith("25 50 44 46")) return "application/pdf";
         if (hex.StartsWith("FF D8 FF")) return "image/jpeg";
         if (hex.StartsWith("89 50 4E 47")) return "image/png";
         if (hex.StartsWith("47 49 46 38")) return "image/gif";
         if (hex.StartsWith("42 4D")) return "image/bmp";
         if (hex.StartsWith("50 4B 03 04")) return "application/zip"; // docx, xlsx, pptx
         if (hex.StartsWith("1F 8B")) return "application/gzip";
         if (hex.StartsWith("52 61 72 21")) return "application/x-rar-compressed";
         if (hex.StartsWith("49 44 33") || hex.StartsWith("FF FB")) return "audio/mpeg";
         if (hex.StartsWith("4F 67 67 53")) return "application/ogg";
         if (hex.StartsWith("52 49 46 46")) return "audio/wav"; // ojo: podría ser AVI también
         if (hex.StartsWith("4D 5A")) return "application/x-msdownload"; // exe, dll
         if (hex.StartsWith("7F 45 4C 46")) return "application/x-executable"; // ELF (Linux binarios)
         if (hex.StartsWith("3C 3F 78 6D 6C")) return "application/xml";
         if (hex.StartsWith("7B 5C 72 74 66")) return "application/rtf";
         if (hex.StartsWith("30 26 B2 75 8E 66 CF 11")) return "video/x-ms-wmv";
         if (hex.StartsWith("D0 CF 11 E0 A1 B1 1A E1")) return "application/vnd.ms-office"; // Word/Excel/PowerPoint clásicos

         return "application/octet-stream";
      }

      /// <summary>
      /// Devuelve un secuencia de mensajes de excepciones en una sola cadena de texto.
      /// </summary>
      /// <param name="xc">Excepción incial.</param>
      /// <returns>Secuencia de mensajes de excepciones en una sola cadena de texto.</returns>
      public static string ObtenerExcepcionMensajes(Exception xc)
      {
         var eslabon = xc;
         txt.StringBuilder sb = new txt.StringBuilder();
         while (!(eslabon is null))
         {
            sb.Append(" / ");
            sb.Append(eslabon.Message);

            eslabon = eslabon.InnerException;
         }

         return sb.Remove(0, 1).ToString();
      }

      /// <summary>
      /// Devuelve un Guid nuevo como cadena de texto.
      /// </summary>
      /// <returns>Guid nuevo como cadena de texto.</returns>
      public static string GuidNuevo()
      {
         var guid = Guid.NewGuid();

         return guid.ToString("D");
      }

      public static tsk.Task LanzarTarea(
         Action<IProgress<AvanceInforme>, thr.CancellationToken> tarea,
         IProgress<AvanceInforme> progreso,
         thr.CancellationToken simbolo)
      {
         return tsk.Task.Run(() =>
         {
            simbolo.ThrowIfCancellationRequested();
            tarea(progreso, simbolo);
         }, simbolo);
      }

      public static tsk.Task<T> LanzarTarea<T>(
         Func<IProgress<AvanceInforme>, thr.CancellationToken, T> tarea,
         IProgress<AvanceInforme> progreso,
         thr.CancellationToken simbolo)
      {
         return tsk.Task.Run(() =>
         {
            simbolo.ThrowIfCancellationRequested();
            return tarea(progreso, simbolo);
         }, simbolo);
      }

      public static tsk.Task LanzarTarea<T>(
         Action<lst.List<T>, IProgress<AvanceInforme>, thr.CancellationToken> tarea,
         lst.List<T> lista,
         IProgress<AvanceInforme> progreso,
         thr.CancellationToken simbolo)
      {
         return tsk.Task.Run(() =>
         {
            simbolo.ThrowIfCancellationRequested();
            tarea(lista, progreso, simbolo);
         }, simbolo);
      }

      /// <summary>
      /// Devuelve un valor con formato y unidad de un tamaño o peso en bytes.
      /// </summary>
      /// <param name="tamaño">Tamaño o peso en bytes.</param>
      /// <returns>Valor con formato y unidad de un tamaño o peso en bytes.</returns>
      public static string PresentarTamaño(long tamaño)
      {
         var factor = 1024.0;
         var unidad = "KB";

         var convertido = Math.Ceiling(tamaño / factor);

         if (convertido > factor * 25)
         {
            convertido = Math.Ceiling(convertido / factor);
            unidad = "MB";
         }

         return $"{convertido:#,0} {unidad}";
         /*
         var umbral = 1501;
         var decimales = 2;
         if (convertido > umbral)
         {
            unidad = "KB";
            convertido = Math.Round(convertido / factor, decimales);
            formato = "#,0.00";
         }
         if (convertido > umbral)
         {
            unidad = "MB";
            convertido = Math.Round(convertido / factor, decimales);
            formato = "#,0.00";
         }
         if (convertido > umbral)
         {
            unidad = "GB";
            convertido = Math.Round(convertido / factor, decimales);
            formato = "#,0.00";
         }
         return $"{convertido.ToString(formato)} {unidad}";*/
      }

      /// <summary>
      /// Lanza una excepción de tipo <see cref="LiamException"/> con un mensaje obtenido a partir de un descriptor de recurso.
      /// </summary>
      /// <param name="recursoDescriptor">Nombre del descriptor del recurso.</param>
      /// <param name="recursoCatalogo">Nombre del catálogo donde se encuentra el recurso.</param>
      /// <param name="excepcionInterna">Excepción que causa la excepción actual, o una referencia nula si no existe.</param>
      /// <returns>Lanza una excepción de tipo <see cref="LiamException"/> con un mensaje obtenido a partir de un descriptor de recurso.</returns>
      public static LiamException LanzarExcepcion(string recursoDescriptor, string recursoCatalogo = null, Exception excepcionInterna = null)
      {
         string mensaje = Supervisor.ObtenerTexto(recursoCatalogo ?? Supervisor.Distintivo, recursoDescriptor);

         return new LiamException(mensaje, excepcionInterna);
      }

      public static void ComoTexto(string documentoNombre, lst.List<Elemento> columnas, lst.List<lst.List<object>> datos)
      {
         string separador = "\t";
         using (var sw = new stm.StreamWriter(documentoNombre, false, txt.Encoding.UTF8))
         {
            sw.WriteLine(string.Join(separador, columnas.Select(e => e.Nombre)));
            foreach (var fila in datos)
            {
               sw.WriteLine(string.Join(separador, fila.Select(campo => (campo ?? string.Empty).ToString())));
            }
         }
      }
   }
}
