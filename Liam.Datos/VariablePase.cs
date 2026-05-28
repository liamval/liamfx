namespace Liam.Datos
{
   /// <summary>
   /// Define el paso del parámetro de la variable al método.
   /// </summary>
   public enum VariablePase
   {
      /// <summary>Dato de ingreso convencional.</summary>
      Predeterminado = 0,

      /// <summary>Dato de ingreso y con refresco de valor.</summary>
      Referencia,

      /// <summary>Dato de respuesta.</summary>
      Retorno
   }
}
