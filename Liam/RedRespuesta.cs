using System;

namespace Liam
{
   /// <summary>
   /// Representa la respuesta de un cliente de red.
   /// </summary>
   public class RedRespuesta<T>
   {
      /// <summary>
      /// Obtiene o establece el indicador de respuesta correcta.
      /// </summary>
      public bool EsCorrecto { get; set; }

      /// <summary>
      /// Obtiene o establece el código de estado Http de la respuesta.
      /// </summary>
      public int Estado { get; set; }

      /// <summary>
      /// Obtiene o establece la carga de valor de la respuesta.
      /// </summary>
      public T Contenido { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="RedRespuesta{T}"/>.
      /// </summary>
      public RedRespuesta()
      {
      }

      /// <summary>
      /// Deuvelve una nueva respuesta con el contenido reasignado.
      /// </summary>
      /// <typeparam name="NT">Nuevo tipo para la respuesta.</typeparam>
      /// <param name="nuevo">Nuevo contenido para la respuesta.</param>
      /// <returns>Una nueva respuesta con el contenido reasignado.</returns>
      public RedRespuesta<NT> Mapear<NT>(NT nuevo)
      {
         return new RedRespuesta<NT>()
         {
            Contenido = nuevo,
            EsCorrecto = this.EsCorrecto,
            Estado = this.Estado
         };
      }

      /// <summary>
      /// Deuvelve una nueva respuesta con el contenido reasignado.
      /// </summary>
      /// <typeparam name="NT">Nuevo tipo para la respuesta.</typeparam>
      /// <param name="mapeador">Método que tarnsforma el contenido en el tipo destino.</param>
      /// <returns>Una nueva respuesta con el contenido reasignado.</returns>
      public RedRespuesta<NT> Mapear<NT>(Func<T, NT> mapeador)
      {
         return new RedRespuesta<NT>()
         {
            Contenido = mapeador(this.Contenido),
            EsCorrecto = this.EsCorrecto,
            Estado = this.Estado
         };
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{this.Estado} ({this.EsCorrecto})";
      }
   }
}
