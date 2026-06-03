using System;

namespace Liam.Datos
{
   /// <summary>
   /// Representa un contenedor de datos secuenciales accesibles por índice o por nombre de columna.
   /// </summary>
   public class FuenteFila
   {
      private readonly FuenteTabla _tabla;
      private readonly object[] _valores;

      protected internal FuenteFila(FuenteTabla tabla)
      {
         _tabla = tabla;
         _valores = new object[_tabla.Columnas.Count];
      }

      public FuenteFila(int capacidad)
      {
         _valores = new object[capacidad];
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteFila"/>.
      /// </summary>
      public FuenteFila(object[] valores)
         : base()
      {
         _valores = valores;
      }

      public object this[int indice]
      {
         get
         {
            return _valores[indice];
         }
         set
         {
            _valores[indice] = value;
         }
      }

      protected internal int ObtenerIndice(string columna)
      {
         if (_tabla is null)
            throw new NullReferenceException($"no existe tabla asociada");

         return _tabla?.Columnas?.IndexOf(columna) ?? -1;
         //for (int i = 0; i < _tabla.Columnas.Count; i++)
         //{
         //   if (_tabla.Columnas[i] == columna)
         //      return i;
         //}
         throw new NullReferenceException($"{columna} no existe");
      }

      public object this[string columnaNombre]
      {
         get
         {
            return _valores[ObtenerIndice(columnaNombre)];
         }
         set
         {
            _valores[ObtenerIndice(columnaNombre)] = value;
         }
      }

      public object[] FilaArray
      {
         get
         {
            return _valores;
         }
      }

      //int index = this.GetIndexByColumnName(columnaNombre);
      //if (index >= 0 && index < this.Count)
      //   return this[index];
      //else
      //   throw new IndexOutOfRangeException($"La columna '{columnaNombre}' no existe en la fila.");
      //int index = this.GetIndexByColumnName(columnaNombre);
      //if (index >= 0 && index < this.Count)
      //   this[index] = value;
      //else
      //   throw new IndexOutOfRangeException($"La columna '{columnaNombre}' no existe en la fila.");
   }
}
