using System;

namespace Liam
{
   /// <summary>
   /// Representa el indice para administrar la posición del valor en un vector de datos.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public sealed class OrdinalAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece el índice en la lista.
      /// </summary>
      public int Indice { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="OrdinalAttribute"/>.
      /// </summary>
      /// <param name="indice">Índice en la lista.</param>
      public OrdinalAttribute(int indice)
      {
         this.Indice = indice;
      }
   }
}
