using System;

namespace Liam
{
   /// <summary>
   /// Reprsenta un elemento para operaciones generales.
   /// </summary>
   [Serializable]
   public class Elemento
   {
      /// <summary>
      /// Obtiene o establece el código del elemento.
      /// </summary>
      public string Codigo { get; set; }

      /// <summary>
      /// Obtiene o establece nombre del elemento.
      /// </summary>
      public string Nombre { get; set; }

      /// <summary>
      /// Obtiene o establece la descripción del elemento.
      /// </summary>
      public string Descripcion { get; set; }

      /// <summary>
      /// Obtiene o establece un numerador #1 asociado al elemento.
      /// </summary>
      public int Numerador1 { get; set; }

      /// <summary>
      /// Obtiene o establece un numerador #2 asociado al elemento.
      /// </summary>
      public int Numerador2 { get; set; }

      /// <summary>
      /// Obtiene o establece un valor adicional #1 de tipo <see cref="string"/>.
      /// </summary>
      public string Adicional1 { get; set; }

      /// <summary>
      /// Obtiene o establece un valor adicional #2 de tipo <see cref="object"/>.
      /// </summary>
      public object Adicional2 { get; set; }

      /// <summary>
      /// Obtiene o establece un valor lógico booleano de tipo <see cref="bool"/>.
      /// </summary>
      public bool Estado { get; set; }

      /// <summary>
      /// Obtiene o establece una lista de elementos dependientes del mismo tipo <see cref="Elemento"/>.
      /// </summary>
      public Listado Dependientes { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Elemento"/>.
      /// </summary>
      public Elemento()
      {
         this.Codigo = string.Empty;
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Elemento"/> con un código y nombre.
      /// </summary>
      /// <param name="codigo">Código del elemento.</param>
      /// <param name="nombre">Nombre del elemento.</param>
      public Elemento(string codigo, string nombre)
      {
         this.Codigo = codigo;
         this.Nombre = nombre;
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{this.Nombre}";
      }
   }
}
