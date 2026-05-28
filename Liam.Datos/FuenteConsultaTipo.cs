namespace Liam.Datos
{
   /// <summary>
   /// Representa los tipos de consultas a ejecutar en la fuente de datos.
   /// </summary>
   public enum FuenteConsultaTipo
   {
      /// <summary>Representa un script SQL.</summary>
      Personalizado = 0,

      /// <summary>Representa un procedimiento almacenado que puede retornar una tabla.</summary>
      Metodo = 1,

      /// <summary>Representa una función que retorna siempre un valor discreto.</summary>
      Funcion = 2/*,
      
      /// <summary>Representa un método dinámico para obtener un registro y mapearlo a un objeto.</summary>
      Objeto = 3*/
   }
}
