using System;

namespace Liam
{
   /// <summary>
   /// Representa las excepciones personalizadas.
   /// </summary>
   [Serializable]
   public class LiamException
      : Exception
   {
      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="LiamException"/>.
      /// </summary>
      public LiamException()
         : base()
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="LiamException"/> con un mensaje de error específico.
      /// </summary>
      /// <param name="message">Mensaje que describe el error.</param>
      public LiamException(string message)
         : base(message)
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="LiamException"/> con un mensaje de error específico y una referencia a la excepción interna causante de esta excepción.
      /// </summary>
      /// <param name="message">Mensaje de error que explica la razón de esta excepción.</param>
      /// <param name="inner">Excepción que causa la excepción actual, o una referencia nula si no existe.</param>
      public LiamException(string message, Exception inner)
         : base(message, inner)
      {
      }
   }
}
