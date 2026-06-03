using System;
using Liam.Datos;
using Liam.Prueba.Unspsc;
using stm = System.IO;
using tsk = System.Threading.Tasks;
using txt = System.Text;

namespace Liam.Prueba
{
   public class Program
   {
      [MTAThread]
      public async static tsk.Task Main(string[] args)
      {
         Console.OutputEncoding = txt.Encoding.UTF8;
         using (var fabrica = new Fabrica())
         {
            Supervisor.Inicializar(fabrica, culturaNombre: "es-PE");

            string proveedor = FuenteOleDb.ObtenerProveedor();
            string origenDatos = stm.Path.Combine(stm.Directory.GetCurrentDirectory(), "UNGM_UNSPSC.xls");

            fabrica.RegistrarUnico<UnspscEnlace>(fbr => new UnspscEnlace($"Provider={proveedor}; Data Source={origenDatos}; Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\""));

            ComponenteRegistro.RegistrarEnsamblado(typeof(Program).Assembly);

            var demo = new MisDatos();
            await demo.Iniciar();
         }
      }
   }
}
