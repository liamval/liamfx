using System;
using lst = System.Collections.Generic;

namespace Liam
{
   /// <summary>
   /// Representa un listado de elementos genéricos.
   /// </summary>
   [Serializable]
   public class Listado
      : lst.List<Elemento>
   {
      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Listado"/>.
      /// </summary>
      public Listado()
         : base()
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Listado"/> con una capacidad inicial.
      /// </summary>
      /// <param name="capacidadInicial"></param>
      public Listado(int capacidadInicial)
         : base(capacidadInicial)
      {
      }
   }
}
