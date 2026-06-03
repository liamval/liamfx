using Liam.Datos;

namespace Liam.Prueba.Unspsc
{
   /// <summary>
   /// Representa un enlace o conexión a la fuente de datos UNSPSC.
   /// </summary>
   public class UnspscEnlace
       : FuenteEnlace
   {
      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="UnspscEnlace"/> con una cadena de conexión.
      /// </summary>
      /// <param name="conexion">Cadena de conexión al catálogo de datos.</param>
      public UnspscEnlace(string conexion)
         : base(FuenteTipo.OleDb, conexion)
      {
      }
   }
}
