using gbl = System.Globalization;

namespace Liam
{
   /// <summary>
   /// Representa un idioma habilitado para el programa.
   /// </summary>
   public class Idioma
   {
      private Idioma() { }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Idioma"/>.
      /// </summary>
      /// <param name="codigo">Código del idioma.</param>
      /// <param name="nombreLocal">Nombre del idioma (en su idioma).</param>
      public Idioma(string codigo, string nombreLocal)
      {
         this.Codigo = codigo;
         this.NombreLocal = nombreLocal;
      }

      /// <summary>
      /// Obtiene o establece el código del idioma.
      /// </summary>
      public string Codigo { get; set; }

      /// <summary>
      /// Obtiene o establece la descripción local del idioma.
      /// </summary>
      public string NombreLocal { get; set; }

      /// <summary>
      /// Obtiene o establece un indicador de idioma principal del programa.
      /// </summary>
      public bool EsPrincipal { get; set; }

      /// <summary>
      /// Obtiene o establece la configuración de cultura del idioma.
      /// </summary>
      public gbl.CultureInfo Configuracion { get; set; }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{this.NombreLocal}";
      }
   }
}
