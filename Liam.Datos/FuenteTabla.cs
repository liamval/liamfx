using System;
using lst = System.Collections.Generic;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una tabla de datos, con filas y columnas.
   /// </summary>
   public class FuenteTabla
   {
      /// <summary>
      /// Obtiene la lista de columnas que componen la tabla.
      /// </summary>
      public FuenteColumnaColeccion Columnas { get; internal set; }

      /// <summary>
      /// Obtiene la lista de filas que componen la tabla.
      /// </summary>
      public lst.List<FuenteFila> Filas { get; internal set; }

      /// <summary>
      /// Obtiene un valor que indica si la tabla ha sido limitada por el número máximo de filas especificado en la consulta.
      /// </summary>
      public bool EstaLimitado { get; internal set; }

      /// <summary>
      /// Obtiene un vector con los valores de la primera fila de la tabla, en el mismo orden que las columnas. Si la tabla no tiene filas, devuelve null (Nothing en Visual Basic).
      /// </summary>
      public object[] Lista0
      {
         get
         {
            if (this.Filas.Count > 0)
               return this.Filas[0].FilaArray;

            return null;
         }
      }

      /// <summary>
      /// Obtiene un diccionario con los valores de la primera fila de la tabla, mapeando el nombre de cada columna con su valor correspondiente. Si la tabla no tiene filas, devuelve null (Nothing en Visual Basic).
      /// </summary>
      public lst.Dictionary<string, object> Fila0
      {
         get
         {
            if (this.Filas.Count > 0)
            {
               var retval = new lst.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
               for (int i = 0; i < this.Columnas.Count; i++)
               {
                  retval[this.Columnas[i].Nombre] = Filas[0][i];
               }
               return retval;
            }

            return null;
         }
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteTabla"/>.
      /// </summary>
      public FuenteTabla()
      {
         this.Inicializar();
      }

      private void Inicializar()
      {
         this.Filas = new lst.List<FuenteFila>();
         this.Columnas = new FuenteColumnaColeccion();
      }

      public FuenteFila CrearFila()
      {
         var fila = new FuenteFila(this);
         return fila;
      }
   }
}
