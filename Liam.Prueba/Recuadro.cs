using System;
using System.Globalization;

namespace Liam.Prueba
{
   /// <summary>
   /// 
   /// </summary>
   /// <remarks>
   /// ┌─┬─┐  ╔═╦═╗  
   /// │ │ │  ║ ║ ║  
   /// ├─┼─┤  ╠═╬═╣  ╟╢
   /// │ │ │  ║ ║ ║  
   /// └─┴─┘  ╚═╩═╝  
   /// </remarks>
   public class Recuadro
   {
      private const int HSize = 100;
      private const string Liner1 = "─┌┐┘└│┼";
      private const string Liner2 = "═╔╗╝╚║╬";

      /// <summary>
      /// 
      /// </summary>
      /// <param name="longitud"></param>
      public static void PintarLinea(int longitud)
      {
         Console.WriteLine($"".PadRight(longitud, Liner1[0]));
      }

      /// <summary>
      /// Muestra un mensaje delineado en consola.
      /// </summary>
      /// <param name="message">Texto a mostrar en el dispositivo de salida</param>
      /// <param name="title">Titulo opcional.</param>
      /// <param name="botones">Se omite el uso de este valor.</param>
      /// <param name="icono">Se omite el uso de este valor.</param>
      public static void Mostrar(string message, string title = null)
      {
         if (title is null)
            title = Supervisor.Nombre;

         string[] lineas = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
         int localHSize = HSize;
         int maxMsgLn = Console.WindowWidth - 2;

         foreach (string linea in lineas)
         {
            if (linea.Length > localHSize - 3)
            {
               localHSize = linea.Length + 3;
            }
         }

         if (localHSize > maxMsgLn)
            localHSize = maxMsgLn;

         int maxTltLn = localHSize / 2;

         if (title.Length > maxTltLn)
            title = title.Substring(0, maxTltLn) + "...";

         Console.WriteLine($"{Liner2[1]}{Liner2[0]}// {title} ".PadRight(localHSize, Liner2[0]) + $"{Liner2[2]}");

         for (int i = 0; i < lineas.Length; i++)
         {
            if (lineas[i].Length > localHSize - 3)
            {
               lineas[i] = lineas[i].Substring(0, localHSize - 6) + "...";
            }
            int extraEspacios = 0;
            foreach (char c in lineas[i])
            {
               extraEspacios += 1 * (OcupaEspacios(c) - 1);
            }
            Console.WriteLine($"{Liner2[5]} {lineas[i]}".PadRight(localHSize - extraEspacios, ' ') + $"{Liner2[5]}");
         }

         Console.WriteLine($"{Liner2[4]}".PadRight(localHSize, Liner2[0]) + $"{Liner2[3]}");
         Console.WriteLine();
      }

      private static int OcupaEspacios(char c)
      {
         if (Console.OutputEncoding.HeaderName != "utf-8")
            return 1;

         //Console.WriteLine($"{c}={CharUnicodeInfo.GetUnicodeCategory(c)}");
         // Detecta caracteres de ancho completo (ej: CJK, emoji, etc.)
         if ((c >= '\u1100' && c <= '\u115F') || // Hangul Jamo
             (c >= '\u2329' && c <= '\u232A') || // CJK Brackets
             (c >= '\u2E80' && c <= '\uA4CF') || // CJK Radicals Supplement - Yi Radicals
             (c >= '\uAC00' && c <= '\uD7AF') || // Hangul Syllables
             (c >= '\uF900' && c <= '\uFAFF') || // CJK Compatibility Ideographs
             (c >= '\uFE10' && c <= '\uFE19') || // Vertical forms
             (c >= '\uFE30' && c <= '\uFE6F') || // CJK Compatibility Forms
             (c >= '\uFF00' && c <= '\uFF60') || // Fullwidth ASCII variants
             (c >= '\uFFE0' && c <= '\uFFE6') || // Fullwidth symbols
                                                 //(c >= '\u20000' && c <= '\u2FFFD') || // CJK Unified Ideographs Extensions
                                                 //(c >= '\u30000' && c <= '\u3FFFD') || // Tertiary Ideographic Plane
             CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherSymbol) // Emoji
         {
            return 2;
         }

         return 1;
      }
   }
}
