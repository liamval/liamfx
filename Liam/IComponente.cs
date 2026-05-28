namespace Liam
{
   /// <summary>
   /// Representa el contrato para un componente.
   /// </summary>
   public interface IComponente
   {
      /// <summary>
      /// Obtiene el valor del emento extendido según su nombre.
      /// </summary>
      /// <param name="nombre">Nombre del elemento extendido.</param>
      /// <returns>Valor del emento extendido.</returns>
      object Obtener(string nombre);

      /// <summary>
      /// Crea o modifica un elemento extendido, con nombre y valor.
      /// </summary>
      /// <param name="nombre">Nombre del elemento extendido.</param>
      /// <param name="valor">Valor para el elemento extendido.</param>
      /// <returns>Referencia a la instancia para encadenar ediciones.</returns>
      IComponente Editar(string nombre, object valor);
   }
}
