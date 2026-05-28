using System;
using dat = System.Data;
using dgx = System.Diagnostics;
using lst = System.Collections.Generic;
using ole = System.Data.OleDb;
using thr = System.Threading;
using tsk = System.Threading.Tasks;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una fuente de datos basada en el proveedor OleDb para documentos Excel y bases de datos Access.
   /// </summary>
   public class FuenteOleDb
     : FuenteConsulta
   {
      /// <summary>
      /// Obtiene o establece el listado de proveedores OleDb conocidos.
      /// </summary>
      public static lst.List<string> OleDbConocidos = new lst.List<string>() {
         "Microsoft.ACE.OLEDB.16.0",
         "Microsoft.ACE.OLEDB.14.0",
         "Microsoft.ACE.OLEDB.12.0",
         "Microsoft.Jet.OLEDB.4.0"
      };

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteOleDb"/> con una cadena de conexión.
      /// </summary>
      /// <param name="enlace">Cadena de conexión hacia la fuente de datos.</param>
      public FuenteOleDb(string enlace)
         : base(enlace, FuenteConsultaTipo.Personalizado)
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteOleDb"/> con una cadena de conexión y un tipo de consulta.
      /// </summary>
      /// <param name="enlace">Cadena de conexión hacia la fuente de datos.</param>
      /// <param name="tipo">Tipo de consulta a realizar.</param>
      public FuenteOleDb(string enlace, FuenteConsultaTipo tipo)
         : base(enlace, tipo)
      {
      }

      private void ComandoRellenar(ole.OleDbCommand comando)
      {
         switch (this.Tipo)
         {
            case FuenteConsultaTipo.Metodo:
            case FuenteConsultaTipo.Funcion:
               comando.CommandType = dat.CommandType.StoredProcedure;
               break;

            case FuenteConsultaTipo.Personalizado:
            default:
               comando.CommandType = dat.CommandType.Text;
               break;
         }

         bool tieneRetorno = false;
         foreach (var p in this.Parametros)
         {
            int longitud = (p.Valor is string || p.Tipo == VariableTipo.Texto) ? p.Longitud : 0;
            var parametro = new ole.OleDbParameter(p.Nombre, p.Valor ?? DBNull.Value);
            switch (p.Pase)
            {
               case VariablePase.Referencia:
                  parametro.Direction = dat.ParameterDirection.InputOutput;
                  break;
               case VariablePase.Retorno:
                  parametro.Direction = dat.ParameterDirection.ReturnValue;
                  tieneRetorno = true;
                  break;
               default:
                  parametro.Direction = dat.ParameterDirection.Input;
                  break;
            }
            if (p.Valor is DateTime)
               parametro.OleDbType = ole.OleDbType.Date;
            if (longitud > 0)
               parametro.Size = longitud;

            comando.Parameters.Add(parametro);
         }

         if (this.Tipo == FuenteConsultaTipo.Metodo && !tieneRetorno)
         {
            this.Parametros.Add(new FuenteParametro("@return_value") { Tipo = VariableTipo.Automatico, Pase = VariablePase.Retorno });
            var retval = new ole.OleDbParameter("@return_value", dat.SqlDbType.Int) { Value = 0, Direction = dat.ParameterDirection.ReturnValue };
            comando.Parameters.Add(retval);
         }
      }

      public override FuenteRespuesta Consultar(int maxTablas = 64, int maxFilas = 0)
      {
         var medidor = new dgx.Stopwatch();
         var respuesta = new FuenteRespuesta();

         using (var conexion = new ole.OleDbConnection(this.Enlace))
         {
            conexion.Open();
            using (var comando = new ole.OleDbCommand(this.Solicitud, conexion))
            {
               this.ComandoRellenar(comando);
               medidor.Start();
               using (var lector = comando.ExecuteReader(dat.CommandBehavior.SequentialAccess))
               {
                  medidor.Stop();
                  Console.Write($"{medidor.ElapsedMilliseconds,3}ms");

                  medidor.Restart();
                  int tablasLeidas = 0;
                  do
                  {
                     if (tablasLeidas++ >= maxTablas)
                        break;
                     var tabla = this.TablaLeer(lector, maxFilas);
                     respuesta.Tablas.Add(tabla);
                  }
                  while (!lector.IsClosed && lector.NextResult());
                  medidor.Stop();
                  Console.Write($"{medidor.ElapsedMilliseconds,3}ms");

                  medidor.Restart();
                  foreach (var p in this.Parametros)
                  {
                     if (p.Pase == VariablePase.Referencia || p.Pase == VariablePase.Retorno)
                     {
                        object valor = comando.Parameters[p.Nombre].Value;

                        if (valor is DBNull)
                           valor = null;

                        if (p.Pase == VariablePase.Retorno)
                           respuesta.Retorno = valor;

                        p.Valor = valor;
                     }
                  }
                  medidor.Stop();
                  Console.Write($"{medidor.ElapsedMilliseconds,3}ms");
               }
            }
         }

         return respuesta;
      }

      public override tsk.Task<FuenteRespuesta> ConsultarAsync(thr.CancellationToken simbolo, int maxTablas = 64, int maxFilas = 0)
      {
         return base.ConsultarAsync(simbolo, maxTablas, maxFilas);
      }

      /// <summary>
      /// Devuelve los nombres de las hojas en un documento Excel o los nombres de las tablas en una base de datos Access.
      /// </summary>
      /// <returns></returns>
      public lst.List<string> ObtenerTablas()
      {
         var tablas = new lst.List<string>();

         using (var conexion = new ole.OleDbConnection(this.Enlace))
         {
            conexion.Open();
            var dt = conexion.GetSchema("Tables");
            foreach (dat.DataRow fila in dt.Rows)
            {
               tablas.Add($"{fila["TABLE_NAME"]}");
            }
         }

         return tablas;
      }

      /// <summary>
      /// Devuelve un proveedor OLEDB registrado en la estación, que figure en la lista <see cref="OleDbConocidos"/> o null (Nothing en Visual Basic) si no hay coincidencias.
      /// </summary>
      /// <returns>Proveedor OLEDB registrado en la estación, que figure en la lista <see cref="OleDbConocidos"/> o null (Nothing en Visual Basic) si no hay coincidencias.</returns>
      public static string ObtenerProveedor()
      {
         ole.OleDbEnumerator dr = new ole.OleDbEnumerator();
         dat.DataTable dt = dr.GetElements();

         foreach (string s in OleDbConocidos)
         {
            foreach (dat.DataRow item in dt.Rows)
            {
               if (s == (string)item[0])
                  return s;
            }
         }

         return null;
      }

      /// <summary>
      /// Devuelve un proveedor OLEDB registrado en la estación, que figure en la lista <see cref="OleDbConocidos"/> o null (Nothing en Visual Basic) si no hay coincidencias.
      /// </summary>
      /// <returns>Proveedor OLEDB registrado en la estación, que figure en la lista <see cref="OleDbConocidos"/> o null (Nothing en Visual Basic) si no hay coincidencias.</returns>
      public static lst.List<string> ObtenerOleDbProveedores(string filtro = ".OLEDB.")
      {
         ole.OleDbEnumerator dr = new ole.OleDbEnumerator();
         dat.DataTable dt = dr.GetElements();

         lst.List<string> proveedores = new lst.List<string>();

         foreach (dat.DataRow item in dt.Rows)
         {
            var nombre = (string)item[0];
            if (nombre.Contains(filtro))
               proveedores.Add(nombre);
         }

         return proveedores;
      }
   }
}
