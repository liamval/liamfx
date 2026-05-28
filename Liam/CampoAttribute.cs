using System;

namespace Liam
{
   /// <summary>
   /// Representa el nombre de la propiedad o columna en el repositorio de datos.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public sealed class CampoAttribute
      : Attribute
   {
      /// <summary>
      /// Obtiene o establece el nombre del campo de datos en el repositorio de datos.
      /// </summary>
      public string Nombre { get; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="CampoAttribute"/>.
      /// </summary>
      /// <param name="nombre">Nombre del campo de datos en el repositorio de datos.</param>
      public CampoAttribute(string nombre)
      {
         this.Nombre = nombre;
      }
   }
}
