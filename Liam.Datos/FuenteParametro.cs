using dat = System.Data;

namespace Liam.Datos
{
   /// <summary>
   /// Representa un parámetro para una variable de una solicitud.
   /// </summary>
   public class FuenteParametro
   {
      /// <summary>
      /// Obtiene o establece el nombre del parámetro.
      /// </summary>
      public string Nombre { get; }

      /// <summary>
      /// Obtiene o establece el valor del parámetro.
      /// </summary>
      public object Valor { get; set; }

      /// <summary>
      /// Obtiene o establece la longitud máxima para un parámetro de tipo texto.
      /// </summary>
      public int Longitud { get; set; }

      /// <summary>
      /// Obtiene o establece el tipo del parámetro.
      /// </summary>
      public VariableTipo Tipo { get; set; }

      /// <summary>
      /// Obtiene o establece el paso del parámetro al método.
      /// </summary>
      public VariablePase Pase { get; set; }

      /// <summary>
      /// Obtiene o establece el objeto subyacente del parámetro.
      /// </summary>
      internal dat.IDbDataParameter ParametroInterno { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteParametro"/>.
      /// </summary>
      private FuenteParametro()
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteParametro"/> con un nombre y valor.
      /// </summary>
      /// <param name="nombre">Nombre del parámetro.</param>
      /// <param name="valor">Valor del parámetro.</param>
      /// <param name="longitud">Longitud para los textos.</param>
      public FuenteParametro(string nombre, object valor = null, int longitud = 400)
      {
         this.Nombre = nombre;
         this.Valor = valor;
         this.Longitud = longitud;
      }

      /// <summary>
      /// Devuelve el nombre del parámetro.
      /// </summary>
      /// <returns>Nombre del parámetro.</returns>
      public override string ToString()
      {
         return $"{this.Nombre}";
      }
   }
}
