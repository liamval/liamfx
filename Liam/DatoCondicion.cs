using System;

namespace Liam
{
   /// <summary>
   /// Representa las condiciones para la transferencia del dato hacia la propiedad.
   /// </summary>
   [Flags]
   public enum DatoCondicion
   {
      /// <summary>Valor predeterminado. No afecta las demás condiciones existentes.</summary>
      Normal = 0,

      /// <summary>Quitar los espacios en blanco al inicio y final del texto.</summary>
      QuitarRelleno = 1,

      /// <summary>Trata un numero como nulo, si es un valor cero.</summary>
      NumeroCeroComoNulo = 2,

      /// <summary>Trata un texto como nulo, si es un texto vacío.</summary>
      TextoVacioComoNulo = 4,

      /// <summary>Convierte el dato en texto (<see cref="string"/>).</summary>
      ComoTexto = 8,

      /// <summary>Convierte el dato en número decimal (<see cref="decimal"/>).</summary>
      ComoNumeroDec = 16,

      /// <summary>Convierte el dato en número entero de 32bit (<see cref="int"/>).</summary>
      ComoNumero32 = 32,

      /// <summary>Convierte el dato en número entero de 64bit (<see cref="long"/>).</summary>
      ComoNumero64 = 64,

      /// <summary>Transforma el texto a mayúsculas.</summary>
      TransformarMayuscula = 128,

      /// <summary>Transforma el texto a minúsculas.</summary>
      TransformarMinuscula = 256
   }
}
