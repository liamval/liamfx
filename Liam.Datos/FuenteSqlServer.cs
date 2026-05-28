using System;
using dat = System.Data;
using dgx = System.Diagnostics;
using sql = System.Data.SqlClient;
using thr = System.Threading;
using tsk = System.Threading.Tasks;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una fuente de datos basada en el proveedor SqlClient para SQL Server.
   /// </summary>
   public class FuenteSqlServer
     : FuenteConsulta
   {
      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteSqlServer"/> con una cadena de conexión.
      /// </summary>
      /// <param name="enlace">Cadena de conexión hacia la fuente de datos.</param>
      public FuenteSqlServer(string enlace)
         : base(enlace, FuenteConsultaTipo.Metodo)
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteSqlServer"/> con una cadena de conexión y un tipo de consulta.
      /// </summary>
      /// <param name="enlace">Cadena de conexión hacia la fuente de datos.</param>
      /// <param name="tipo">Tipo de consulta a realizar.</param>
      public FuenteSqlServer(string enlace, FuenteConsultaTipo tipo)
         : base(enlace, tipo)
      {
      }

      private void ComandoRellenar(sql.SqlCommand comando)
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
         foreach (FuenteParametro p in this.Parametros)
         {
            int longitud = (p.Valor is string || p.Tipo == VariableTipo.Texto) ? p.Longitud : 0;
            var parametro = new sql.SqlParameter(p.Nombre, p.Valor ?? DBNull.Value);

            switch(p.Pase)
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
               parametro.SqlDbType = dat.SqlDbType.DateTime;
            if (longitud > 0)
               parametro.Size = longitud;

            comando.Parameters.Add(parametro);
         }

         if (this.Tipo == FuenteConsultaTipo.Metodo && !tieneRetorno)
         {
            this.Parametros.Add(new FuenteParametro("@return_value") { Tipo = VariableTipo.Automatico, Pase = VariablePase.Retorno });
            var retval = new sql.SqlParameter("@return_value", dat.SqlDbType.Int) { Value = 0, Direction = dat.ParameterDirection.ReturnValue };
            comando.Parameters.Add(retval);
         }
      }

      public override FuenteRespuesta Consultar(int maxTablas = 64, int maxFilas = 0)
      {
         var medidor = new dgx.Stopwatch();
         var respuesta = new FuenteRespuesta();

         using (var conexion = new sql.SqlConnection(this.Enlace))
         {
            conexion.Open();
            using (var comando = new sql.SqlCommand(this.Solicitud, conexion))
            {
               this.ComandoRellenar(comando);
               medidor.Start();
               using (var lector = comando.ExecuteReader(dat.CommandBehavior.SequentialAccess))
               {
                  medidor.Stop();
                  Console.Write($"{medidor.ElapsedMilliseconds, 3}ms");

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

      public override async tsk.Task<FuenteRespuesta> ConsultarAsync(thr.CancellationToken simbolo, int maxTablas = 64, int maxFilas = 0)
      {
         var medidor = new dgx.Stopwatch();
         var respuesta = new FuenteRespuesta();
         try
         {
            using (var conexion = new sql.SqlConnection(this.Enlace))
            {
               await conexion.OpenAsync(simbolo).ConfigureAwait(false);
               using (var comando = new sql.SqlCommand(this.Solicitud, conexion))
               {
                  this.ComandoRellenar(comando);
                  medidor.Start();
                  using (var lector = await comando.ExecuteReaderAsync(dat.CommandBehavior.SequentialAccess, simbolo).ConfigureAwait(false))
                  {
                     medidor.Stop();
                     Console.Write($"{medidor.ElapsedMilliseconds,3}ms");

                     int tablasLeidas = 0;
                     do
                     {
                        if (tablasLeidas++ >= maxTablas)
                           break;
                        medidor.Restart();
                        var tabla = await this.TablaLeerAsync(lector, maxFilas, simbolo);
                        medidor.Stop();
                        Console.Write($"{medidor.ElapsedMilliseconds,3}ms");
                        respuesta.Tablas.Add(tabla);
                     }
                     while (!lector.IsClosed && await lector.NextResultAsync(simbolo).ConfigureAwait(false) && !simbolo.IsCancellationRequested);

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
         }
         catch (OperationCanceledException)
         {
         }

         return respuesta;
      }

      internal async tsk.Task<FuenteTabla> TablaLeerAsync(sql.SqlDataReader lector, int maxFilas, thr.CancellationToken simbolo)
      {
         var tabla = new FuenteTabla();
         int filaContador = 0;
         int cantidadColumnas = lector.FieldCount;

         for (int i = 0; i < cantidadColumnas; i++)
         {
            var columna = new FuenteColumna()
            {
               Ordinal = i,
               Nombre = lector.GetName(i),
               Tipo = lector.GetFieldType(i),
               TipoNativo = lector.GetDataTypeName(i),
            };
            tabla.Columnas.Add(columna);
         }

         while (maxFilas == 0 || filaContador < maxFilas)
         {
            bool tieneFila = await lector.ReadAsync(simbolo).ConfigureAwait(false);

            if (!tieneFila)
               break;

            filaContador++;
            var fila = tabla.CrearFila();
            for (int i = 0; i < cantidadColumnas; i++)
            {
               object campoValor = lector.IsDBNull(i) ? null : lector.GetValue(i);
               fila[i] = campoValor;
            }
            tabla.Filas.Add(fila);

            if (simbolo.IsCancellationRequested)
               break;
         }
         tabla.EstaLimitado = (maxFilas > 0 && filaContador >= maxFilas) || simbolo.IsCancellationRequested;

         return tabla;
      }
   }
}
