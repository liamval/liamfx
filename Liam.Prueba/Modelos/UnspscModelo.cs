using System;

namespace Liam.Prueba
{
   [Modelo("Unspsc")]
   public class UnspscModelo
      : Componente
   {
      [Clave]
      [Campo("Key")]
      [Condiciones(DatoCondicion.ConvertirANumero32)]
      public int Key { get; set; }

      [Campo("Parent key")]
      [Condiciones(DatoCondicion.ConvertirANumero32)]
      public int? ParentKey { get; set; }

      [Campo("Code")]
      [Condiciones(DatoCondicion.ConvertirATexto)]
      public string Code { get; set; }

      [Campo("Title")]
      [Condiciones(DatoCondicion.ConvertirATexto)]
      public string Title { get; set; }
   }
}
