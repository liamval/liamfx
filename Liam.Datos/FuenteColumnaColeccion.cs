using clc = System.Collections;
using lst = System.Collections.Generic;

namespace Liam.Datos
{
   public class FuenteColumnaColeccion
      : lst.IReadOnlyCollection<FuenteColumna>, lst.ICollection<FuenteColumna>, lst.IEnumerable<FuenteColumna>
   {
      public int Count => _columnas.Count;

      public bool IsReadOnly => false;

      private readonly lst.List<FuenteColumna> _columnas;
      private readonly lst.Dictionary<string, int> _mapa;

      public FuenteColumnaColeccion()
      {
         _columnas = new lst.List<FuenteColumna>();
         _mapa = new lst.Dictionary<string, int>();
      }

      public FuenteColumnaColeccion(lst.IEnumerable<FuenteColumna> columnas)
      {
         _columnas = new lst.List<FuenteColumna>(columnas);
         this.ReconstruirIndice();
      }

      public FuenteColumnaColeccion(params FuenteColumna[] columnas)
      {
         _columnas = new lst.List<FuenteColumna>(columnas);
         this.ReconstruirIndice();
      }

      public FuenteColumna this[int index] => _columnas[index];

      public FuenteColumna this[string nombre] => this[_mapa[nombre]];

      public int IndexOf(FuenteColumna columna)
      {
         return IndexOf(columna.Nombre);
         //return _columnas.IndexOf(item);
      }

      public int IndexOf(string nombre)
      {
         return _mapa.TryGetValue(nombre, out int i) ? i : -1;
      }

      public void Insert(int index, FuenteColumna item)
      {
         _columnas.Insert(index, item);
         this.ReconstruirIndice();
      }

      public void Add(FuenteColumna item)
      {
         _columnas.Add(item);
         _mapa[item.Nombre] = _columnas.Count - 1;
      }

      public void Clear()
      {
         _columnas.Clear();
         _mapa.Clear();
      }

      public bool Contains(FuenteColumna item)
      {
         return _columnas.Contains(item);
      }

      public void CopyTo(FuenteColumna[] array, int arrayIndex)
      {
         _columnas.CopyTo(array, arrayIndex);
      }

      public lst.IEnumerator<FuenteColumna> GetEnumerator()
      {
         return _columnas.GetEnumerator();
      }

      public bool Remove(FuenteColumna item)
      {
         return _columnas.Remove(item);
      }

      clc.IEnumerator clc.IEnumerable.GetEnumerator()
      {
         return this.GetEnumerator();
      }

      private void ReconstruirIndice()
      {
         for (int i = 0; i < _columnas.Count; i++)
         {
            _mapa[_columnas[i].Nombre] = i;
         }
      }
   }
}
