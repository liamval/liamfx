using lst = System.Collections.Generic;

namespace Liam
{
   public class Busqueda
   {
      public lst.List<IComponente> Resultados { get; set; }

      public Busqueda()
      {
         this.Resultados = new lst.List<IComponente>();
      }

      public override string ToString()
      {
         return $"{this.Resultados?.Count ?? 0} elemento(s)";
      }
   }
}
