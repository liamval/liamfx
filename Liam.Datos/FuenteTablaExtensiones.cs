using dat = System.Data;

namespace Liam.Datos
{
   public static class FuenteTablaExtensiones
   {
      public static dat.DataTable ComoDataTable(this FuenteTabla tabla)
      {
         var destino = new dat.DataTable();
         var lista = tabla.Filas;

         if (lista is null || lista.Count == 0)
            return destino;

         foreach (var columna in tabla.Columnas)
            destino.Columns.Add(columna.Nombre, typeof(object));

         foreach (var dict in lista)
         {
            var fila = destino.NewRow();
            for (int i = 0; i < tabla.Columnas.Count; i++)
               fila[i] = dict[i];

            destino.Rows.Add(fila);
         }

         return destino;
      }
   }
}
