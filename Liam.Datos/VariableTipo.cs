namespace Liam.Datos
{
   /// <summary>
   /// Contiene los tipos de datos disponibles para delimitar el alcance de las variables.
   /// </summary>
   public enum VariableTipo
   {
      /// <summary>Se toma el tipo de dato del valor del parámetro.</summary>
      Automatico = 0,

      /// <summary>Indica que es texto.</summary>
      Texto,

      /// <summary>Indica que es un número.</summary>
      Numero
   }
}
