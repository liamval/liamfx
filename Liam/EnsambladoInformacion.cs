namespace Liam
{
   /// <summary>
   /// Representa la información de un ensamblado.
   /// </summary>
   public class EnsambladoInformacion
   {
      /// <summary>
      /// Obtiene el nombre del ensamblado.
      /// </summary>
      public string Nombre { get; internal set; }

      /// <summary>
      /// Obtiene la versión del ensamblado.
      /// </summary>
      public string Version { get; internal set; }

      /// <summary>
      /// Obtiene el nombre del producto propietario del ensamblado.
      /// </summary>
      public string Producto { get; internal set; }

      /// <summary>
      /// Obtiene el nombre de la organización propietaria del producto.
      /// </summary>
      public string Organizacion { get; internal set; }

      /// <summary>
      /// Obtiene la descripción del ensamblado.
      /// </summary>
      public string Descripcion { get; internal set; }

      /// <summary>
      /// Obtiene los derechos de copia del ensamblado.
      /// </summary>
      public string Derechos { get; internal set; }

      /// <summary>
      /// Obtiene el código del idioma del ensamblado.
      /// </summary>
      public string IdiomaCodigo { get; internal set; }

      /// <summary>
      /// Obtiene el nombre del módulo del ensamblado.
      /// </summary>
      public string ModuloNombre { get; internal set; }

      /// <summary>
      /// Obtiene el Id del módulo del ensamblado.
      /// </summary>
      public string ModuloId { get; internal set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="EnsambladoInformacion"/>.
      /// </summary>
      public EnsambladoInformacion()
      {
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return this.ToString(false, true);
      }

      /// <summary>
      /// Devuelve el producto (según indicador de <paramref name="mostrarProducto"/>) y título del ensamblado (nombre y versión).
      /// </summary>
      /// <param name="mostrarProducto">indicador para mostrar el nombre del producto.</param>
      /// <param name="mostrarIdioma">indicador para mostrar el código de idioma, si es que tiene.</param>
      /// <returns>Producto (según indicador de <paramref name="mostrarProducto"/>) y título del ensamblado (nombre y versión).</returns>
      public string ToString(bool mostrarProducto, bool mostrarIdioma)
      {
         string producto = mostrarProducto ? $"{this.Producto} " : string.Empty;
         string idioma = mostrarIdioma && !string.IsNullOrWhiteSpace(this.IdiomaCodigo) ? $" ({this.IdiomaCodigo})" : string.Empty;

         return $"{producto}{this.Nombre}{idioma} {this.Version}";
      }
   }
}
