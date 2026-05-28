namespace Liam
{
   /// <summary>
   /// Representa el informe de avance en un reporte de progreso.
   /// </summary>
   public class AvanceInforme
   {
      public int Contador { get; set; }

      public IComponente Componente { get; set; }

      public bool TieneTotal { get; set; }

      public int Total { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="AvanceInforme"/>.
      /// </summary>
      public AvanceInforme()
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="AvanceInforme"/> con datos del avance.
      /// </summary>
      /// <param name="componente">Instancia del último componente procesado en el avance.</param>
      /// <param name="contador">Indicador de la cantidad de componentes ya procesados.</param>
      /// <param name="total">Indicador de la cantidad total de componentes.</param>
      public AvanceInforme(IComponente componente, int contador, int total)
      {
         if (total < 0) total = 0;
         if (contador < 0) contador = 0;

         this.Componente = componente;
         this.Contador = contador;
         this.Total = total;
         this.TieneTotal = total > 0;
      }
   }
}
