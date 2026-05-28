using ole = System.Data.OleDb;
using sql = System.Data.SqlClient;

namespace Liam.Datos
{
   /// <summary>
   /// Contiene los tipos de fuentes de datos.
   /// </summary>
   public enum FuenteTipo
   {
      /// <summary>
      /// No hay fuente de datos.
      /// </summary>
      Texto = 0,

      /// <summary>
      /// Optimiza las instancias de conexión y consulta para SQL Server.
      /// </summary>
      [FuenteDatos(typeof(sql.SqlConnection), typeof(sql.SqlCommand), typeof(sql.SqlDataReader))]
      SqlServer = 1,

      /// <summary>
      /// Optimiza las instancias de conexión y consulta para Office Access y Office Excel.
      /// </summary>
      [FuenteDatos(typeof(ole.OleDbConnection), typeof(ole.OleDbCommand), typeof(ole.OleDbDataReader))]
      OleDb = 2
   }
}
