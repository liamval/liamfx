using System;
using System.IO;
using Liam.Datos;
using tsk = System.Threading.Tasks;
using txt = System.Text;

namespace Liam.Prueba
{
   public class Program
   {
      public async static tsk.Task Main(string[] args)
      {
         Console.OutputEncoding = txt.Encoding.UTF8;

         using (var fabrica = new Fabrica())
         {
            Supervisor.Inicializar(fabrica, culturaNombre: "es-PE");
            string proveedor = FuenteOleDb.ObtenerProveedor();
            fabrica.RegistrarUnico<UnspscEnlace>(fbr => new UnspscEnlace($"Provider={proveedor}; Data Source={Path.Combine(Directory.GetCurrentDirectory(), "UNGM_UNSPSC.xls")}; Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\""));

            ComponenteRegistro.RegistrarEnsamblado(typeof(Program).Assembly);

            var demo = new MisDatos();
            await demo.Iniciar();
         }
      }
   }
}
