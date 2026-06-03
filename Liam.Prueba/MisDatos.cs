using System;
using Liam.Datos;
using Liam.Prueba.Unspsc;
using Liam.Prueba.Unspsc.Modelos;
using dgx = System.Diagnostics;
using thr = System.Threading;
using tsk = System.Threading.Tasks;

namespace Liam.Prueba
{
   public class MisDatos
   {
      private readonly UnspscEnlace _enlace = Supervisor.Fabrica.Crear<UnspscEnlace>();

      public async tsk.Task Iniciar()
      {
         string listar1 =
            "select top 1 * from [UNSPSC$]";
         string listar100 =
            "select top 100 * from [UNSPSC$]";
         string listar4000 =
            "select top 2000 * from [UNSPSC$] " +
            "union all " +
            "select top 2000 * from [UNSPSC$] ";

         await ObtenerDatos(listar1); // calentar

         Recuadro.Mostrar($"{Supervisor.Ejecutable.Nombre} {Supervisor.Ejecutable.Version}", "Liam");

         Console.WriteLine("\nObtener y contruir 1 dato");
         Console.WriteLine($"{"Método",50}{"Consulta",20}{"Construcción",20}");
         Recuadro.PintarLinea(110);
         await ObtenerDatos(listar1);
         await ObtenerDatos(listar1);
         Console.WriteLine("\nObtener y contruir 100 datos");
         Console.WriteLine($"{"Método",50}{"Consulta",20}{"Construcción",20}");
         Recuadro.PintarLinea(110);
         await ObtenerDatos(listar100);
         await ObtenerDatos(listar100);

         Console.WriteLine("\nObtener y contruir 4000 datos");
         Console.WriteLine($"{"Método",50}{"Consulta",20}{"Construcción",20}");
         Recuadro.PintarLinea(110);
         await ObtenerDatos(listar4000);
         await ObtenerDatos(listar4000);
      }

      public async tsk.Task ObtenerDatos(string q)
      {
         Console.Write($"{"Componente",30}");
         var comando = new FuenteOleDb(_enlace.Conexion, FuenteConsultaTipo.Personalizado) { Solicitud = q };
         var sw = dgx.Stopwatch.StartNew();
         var respuesta = await comando.ConsultarAsync(thr.CancellationToken.None);
         sw.Stop();
         Console.Write($"{sw.ElapsedMilliseconds,18}ms");
         sw.Restart();
         var modelado = respuesta.ListaConstruir<UnspscModelo>();
         sw.Stop();
         Console.Write($"{sw.ElapsedMilliseconds,18}ms");
         Console.Write($"{modelado.Count,15} modelos");
         Console.WriteLine();
      }
   }
}
