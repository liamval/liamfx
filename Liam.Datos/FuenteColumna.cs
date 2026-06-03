using System;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una columna de una fuente de datos, con su nombre, tipo y orden.
   /// </summary>
   public class FuenteColumna
   {
      public int Ordinal { get; set; }

      public string Nombre { get; set; }

      public Type Tipo { get; set; }

      public string TipoNativo { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteColumna"/>.
      /// </summary>
      public FuenteColumna()
      {
      }

      /// <summary>
      /// Devuelve una representación en cadena de la columna, incluyendo su orden, nombre, tipo y tipo nativo.
      /// </summary>
      /// <returns>Representación en cadena de la columna</returns>
      public override string ToString()
      {
         return $"[{Ordinal}] {Nombre} ({Tipo.Name}) ({TipoNativo})";
      }
   }
}
