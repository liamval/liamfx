namespace Liam.Prueba.Unspsc.Modelos
{
   /// <summary>
   /// Representa un modelo de datos para la clasificación UNSPSC.
   /// </summary>
   /// <remarks>
   /// https://www.ungm.org/Public/UNSPSC
   /// What are the UNSPSC codes?
   /// The United Nations Standard Products and Services Code® (UNSPSC®) is a global classification system of products and services.
   /// These codes are used to classify products and services: in the case of suppliers, to classify the products and services of their company,
   /// and in the case of Staff Members, to classify the products and services when publishing procurement opportunities.
   /// </remarks>
   [Modelo("Unspsc")]
   public class UnspscModelo
      : Componente
   {
      [Clave]
      [Campo("Key")]
      [Condiciones(DatoCondicion.ComoNumero32)]
      public int Key { get; set; }

      [Campo("Parent key")]
      [Condiciones(DatoCondicion.ComoNumero32)]
      public int? ParentKey { get; set; }

      [Campo("Code")]
      [Condiciones(DatoCondicion.ComoTexto)]
      public string Code { get; set; }

      [Campo("Title")]
      [Condiciones(DatoCondicion.ComoTexto)]
      public string Title { get; set; }
   }
}
