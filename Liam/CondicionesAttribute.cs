using System;

namespace Liam
{
   /// <summary>
   /// Representa las condiciones para la carga de un dato hacia su propiedad asociada.
   /// </summary>
   /// <remarks>
   /// * Primero hace las conversiones.<br/>
   /// * Quita rellenos.<br/>
   /// * Tratamiento de valores.<br/>
   /// </remarks>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public sealed class CondicionesAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece las condiciones de carga del dato hacia su propiedad asociada.
      /// </summary>
      public DatoCondicion Condiciones { get; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="CondicionesAttribute"/>.
      /// </summary>
      /// <param name="condiciones">Contiene las condiciones para el tratamiento del dato.</param>
      public CondicionesAttribute(DatoCondicion condiciones)
      {
         this.Condiciones = condiciones;
      }
   }
}
