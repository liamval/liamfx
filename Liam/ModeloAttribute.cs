using System;

namespace Liam
{
   /// <summary>
   /// Representa la asignación del nombre del repositorio o tabla de la fuente de datos.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
   public sealed class ModeloAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece el nombre de la tabla en el repositorio de datos.
      /// </summary>
      public string Nombre { get; }

      /// <summary>
      /// Obtiene o establece el nombre del esquema u otro prefijo separado por un punto.
      /// </summary>
      public string Prefijo { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ModeloAttribute"/>.
      /// </summary>
      /// <param name="nombre">nombre de la tabla en el repositorio de datos.</param>
      public ModeloAttribute(string nombre)
      {
         this.Nombre = nombre;
      }
   }
}
