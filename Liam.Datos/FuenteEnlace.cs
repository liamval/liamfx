namespace Liam.Datos
{
   /// <summary>
   /// Representa un enlace o conexión a una fuente de datos.
   /// </summary>
   public abstract class FuenteEnlace
   {
      /// <summary>
      /// Obtiene o establece el origen o la fuente de datos.
      /// </summary>
      public FuenteTipo Tipo { get; set; }

      /// <summary>
      /// Obtiene o establece los pares de nombre y valor para la conexión a una fuente de datos.
      /// </summary>
      public string Conexion { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteEnlace"/> con una cadena de conexión.
      /// </summary>
      /// <param name="fuenteTipo">Tipo de fuente a utilizar.</param>
      /// <param name="conexion">Cadena de conexión a la fuentre de datos.</param>
      public FuenteEnlace(FuenteTipo fuenteTipo, string conexion)
      {
         this.Tipo = fuenteTipo;
         this.Conexion = conexion;
      }
   }
}
