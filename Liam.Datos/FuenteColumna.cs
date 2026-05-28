using System;
using clc = System.Collections;
using lst = System.Collections.Generic;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una columna de una fuente de datos, con su nombre, tipo y orden.
   /// </summary>
   public class FuenteColumna
   {
      public int Ordinal { get; set; }

      public string Nombre { get; set; }

      public Type Tipo { get; set; }

      public string TipoNativo { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteColumna"/>.
      /// </summary>
      public FuenteColumna()
      {
      }

      /// <summary>
      /// Devuelve una representación en cadena de la columna, incluyendo su orden, nombre, tipo y tipo nativo.
      /// </summary>
      /// <returns>Representación en cadena de la columna</returns>
      public override string ToString()
      {
         return $"[{Ordinal}] {Nombre} ({Tipo.Name}) ({TipoNativo})";
      }
   }

   public class ColumnaColeccion
      : lst.IReadOnlyCollection<FuenteColumna>, lst.ICollection<FuenteColumna>, lst.IEnumerable<FuenteColumna>
   {
      public int Count => _columnas.Count;

      public bool IsReadOnly => false;

      private readonly lst.List<FuenteColumna> _columnas;
      private readonly lst.Dictionary<string, int> _mapa;

      public ColumnaColeccion()
      {
         _columnas = new lst.List<FuenteColumna>();
         _mapa = new lst.Dictionary<string, int>();
      }

      public ColumnaColeccion(lst.IEnumerable<FuenteColumna> columnas)
      {
         _columnas = new lst.List<FuenteColumna>(columnas);
         this.ReconstruirIndice();
      }

      public ColumnaColeccion(params FuenteColumna[] columnas)
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
