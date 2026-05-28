using dat = System.Data;
using lst = System.Collections.Generic;
using thr = System.Threading;
using tsk = System.Threading.Tasks;

namespace Liam.Datos
{
   /// <summary>
   /// Representa una consulta hacia una fuente de datos a través de un mecanismo de obtención de datos.
   /// </summary>
   /// <remarks>
   /// Esta clase sirve como base para la implementación de fuentes de datos especializadas, como SQL Server, OleDb, etc. 
   /// </remarks>
   public abstract class FuenteConsulta
   {
      public string Enlace { get; }

      public FuenteConsultaTipo Tipo { get; set; }

      public string Solicitud { get; set; }

      public lst.List<FuenteParametro> Parametros { get; set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="FuenteConsulta"/> con una cadena de conexión.
      /// </summary>
      /// <param name="enlace">cadena de conexión a la base de datos.</param>
      /// <param name="tipo">Tipo de consulta para realizar.</param>
      public FuenteConsulta(string enlace, FuenteConsultaTipo tipo)
      {
         this.Parametros = new lst.List<FuenteParametro>();
         this.Enlace = enlace;
         this.Tipo = tipo;
      }

      /// <summary>
      /// Devuelve el valor de un parámetro específico por su nombre. Si el parámetro no existe, devuelve null (Nothing en Visual Basic).
      /// </summary>
      /// <param name="parametroNombre">Nombre del parámetro.</param>
      /// <returns>Valor de un parámetro específico por su nombre. Si el parámetro no existe, devuelve null (Nothing en Visual Basic).</returns>
      public object ParametroValor(string parametroNombre)
      {
         foreach (var p in this.Parametros)
         {
            if (p.Nombre == parametroNombre)
            {
               return p.Valor;
            }
         }
         return null;
      }

      /// <summary>
      /// Devuelve el valor del parámetro de retorno de la consulta. Si no existe un parámetro de retorno, devuelve null (Nothing en Visual Basic).
      /// </summary>
      /// <returns>Valor del parámetro de retorno de la consulta.</returns>
      public object RetornoValor()
      {
         foreach (var p in this.Parametros)
         {
            if (p.Pase == VariablePase.Retorno)
            {
               return p.Valor;
            }
         }
         return null;
      }

      /// <summary>
      /// Devuelve el resultado de la consulta a la fuente de datos, limitado por el número máximo de tablas y filas especificado. Si maxTablas o maxFilas es 0, no se aplicará ningún límite en ese aspecto.
      /// </summary>
      /// <param name="maxTablas">Máxima cantidad de tablas a extraer.</param>
      /// <param name="maxFilas">Máxima cantidad de filas a extraer para la tabla.</param>
      /// <returns>Resultado de la consulta a la fuente de datos.</returns>
      public abstract FuenteRespuesta Consultar(int maxTablas, int maxFilas);

      /// <summary>
      /// Devuelve el resultado de la consulta a la fuente de datos de forma asíncrona, limitado por el número máximo de tablas y filas especificado. Si maxTablas o maxFilas es 0, no se aplicará ningún límite en ese aspecto.
      /// </summary>
      /// <param name="simbolo">Símbolo para cancelación de operación asíncrona.</param>
      /// <param name="maxTablas">Máxima cantidad de tablas a extraer.</param>
      /// <param name="maxFilas">Máxima cantidad de filas a extraer para la tabla.</param>
      /// <returns>Resultado de la consulta a la fuente de datos.</returns>
      public virtual tsk.Task<FuenteRespuesta> ConsultarAsync(thr.CancellationToken simbolo, int maxTablas, int maxFilas)
      {
         return tsk.Task.Run(() => this.Consultar(maxTablas, maxFilas), simbolo);
      }

      /// <summary>
      /// Devuelve una tabla de datos a partir de un lector de datos, limitado por el número máximo de filas especificado. Si maxFilas es 0, no se aplicará ningún límite en ese aspecto.
      /// </summary>
      /// <typeparam name="T">Tipo que implementa un lector de datos.</typeparam>
      /// <param name="lector">Instancia que sirve para leer los datos desde la fuente de datos.</param>
      /// <param name="maxFilas">Máxima cantidad de filas a extraer para la tabla.</param>
      /// <returns></returns>
      internal FuenteTabla TablaLeer<T>(T lector, int maxFilas)
         where T : dat.IDataReader
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
            bool tieneFila = lector.Read();

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
         }
         tabla.EstaLimitado = maxFilas > 0 && filaContador >= maxFilas;

         return tabla;
      }

      //internal FuenteDatosAttribute ObtenerDatos(FuenteTipo tipo)
      //{
      //   var campo = typeof(FuenteTipo).GetField(tipo.ToString());
      //   return System.Linq.Enumerable.Cast<FuenteDatosAttribute>(
      //      Attribute.GetCustomAttributes(campo, typeof(FuenteDatosAttribute)))
      //      .FirstOrDefault();

      //   //return campo
      //   //   ?.GetCustomAttributes(typeof(FuenteDatosAttribute), false)
      //   //   .Cast<FuenteDatosAttribute>()
      //   //   .FirstOrDefault();
      //}
   }
}
