using System;

namespace Liam
{
   /// <summary>
   /// Representa el valor para asignar a la propiedad cuando el valor del fuente de datos es <see cref="DBNull"/> o null (Nothing en Visual Basic).
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public sealed class ValorPredefinidoAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece el valor a asignar cuando la propiedad es null.
      /// </summary>
      public object Valor { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ValorPredefinidoAttribute"/>.
      /// </summary>
      /// <param name="valor">Valor a asignar cuando la propiedad es null.</param>
      public ValorPredefinidoAttribute(object valor)
      {
         this.Valor = valor;
      }
   }
}
