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

   /// <summary>
   /// Representa un enlace o conexión a la fuente de datos UNSPSC.
   /// </summary>
   /// <remarks>
   /// https://www.ungm.org/Public/UNSPSC
   /// What are the UNSPSC codes?
   /// The United Nations Standard Products and Services Code® (UNSPSC®) is a global classification system of products and services.
   /// These codes are used to classify products and services: in the case of suppliers, to classify the products and services of their company,
   /// and in the case of Staff Members, to classify the products and services when publishing procurement opportunities.
   /// </remarks>
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
