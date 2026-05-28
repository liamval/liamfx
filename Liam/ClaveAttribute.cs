using System;

namespace Liam
{
   /// <summary>
   /// Representa el campo para usarse como identificador con un índice para composición.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public sealed class ClaveAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece el índice para composición de la clave de identificación.
      /// </summary>
      public int Ordinal { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ClaveAttribute"/>.
      /// </summary>
      /// <param name="ordinal">Índice para composición de la clave de identificación.</param>
      public ClaveAttribute(int ordinal = 0)
      {
         this.Ordinal = ordinal;
      }
   }
}
