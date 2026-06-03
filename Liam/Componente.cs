using System;
using lst = System.Collections.Generic;

namespace Liam
{
   /// <summary>
   /// Representa el objeto base para interacción entre componentes del programa.
   /// </summary>
   [Serializable]
   public class Componente
      : IComponente
   {
      [NonSerialized]
      private readonly lst.Dictionary<string, object> misDatos;

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Componente"/>.
      /// </summary>
      public Componente()
      {
         misDatos = new lst.Dictionary<string, object>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="nombre"></param>
      /// <returns></returns>
      public object Obtener(string nombre)
      {
         return misDatos.TryGetValue(nombre, out object valor) ? valor : null;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="nombre"></param>
      /// <param name="valor"></param>
      /// <returns></returns>
      public IComponente Editar(string nombre, object valor)
      {
         misDatos[nombre] = valor;
         return this;
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{nameof(Componente)}";
      }
   }
}
