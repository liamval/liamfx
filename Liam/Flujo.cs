using System;
using System.Linq;
using lst = System.Collections.Generic;
using tsk = System.Threading.Tasks;

namespace Liam
{
   public class Flujo<TContexto>
   {
      private readonly lst.List<Func<TContexto, Func<tsk.Task>, tsk.Task>> _pasos = new lst.List<Func<TContexto, Func<tsk.Task>, tsk.Task>>();

      public Flujo<TContexto> Usar(Func<TContexto, Func<tsk.Task>, tsk.Task> paso)
      {
         _pasos.Add(paso);

         return this;
      }

      public tsk.Task Ejecutar(TContexto contexto)
      {
         Func<tsk.Task> siguiente = () => tsk.Task.CompletedTask;

         foreach (var paso in _pasos.AsEnumerable().Reverse())
         {
            var current = siguiente;
            siguiente = () => paso(contexto, current);
         }

         return siguiente();
      }
   }
}
