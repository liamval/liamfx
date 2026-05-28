using System;
using lst = System.Collections.Generic;
using rfl = System.Reflection;
using txt = System.Text;

namespace Vesalio.Susan.Datos
{
   /// <summary>
   /// Representa un controlador de consultas para un origen de datos.
   /// </summary>
   public class FuenteAdministrador
   {
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="configuracion"></param>
      /// <param name="claves"></param>
      /// <returns></returns>
      public T DesdeRepositorio<T>(FuenteConfiguracion configuracion, object[] claves) where T : Componente, new()
      {
         if (claves is null || claves.Length == 0)
         {
            throw new SusanException("TAREA: Sin claves de lectura");
         }

         Type clase = typeof(T);
         ModeloAttribute modelo = null;
         object[] atrs = clase.GetCustomAttributes(false);
         foreach (object atr in atrs)
         {
            if (atr is ModeloAttribute m)
            {
               modelo = m;
            }
         }

         rfl.PropertyInfo[] propLista = clase.GetProperties();

         var propClaves = new lst.List<ArregloClave>();
         for (int i = 0; i < propLista.Length; i++)
         {
            rfl.PropertyInfo propSel = propLista[i];
            object[] atribLista = propSel.GetCustomAttributes(true);
            ClaveAttribute claveAtrr = null;
            CampoAttribute campoAttr = null;

            for (int j = 0; j < atribLista.Length; j++)
            {
               if (atribLista[j] is ClaveAttribute clv)
               {
                  claveAtrr = clv;
               }
               if (atribLista[j] is CampoAttribute cmp)
               {
                  campoAttr = cmp;
               }
            }
            if (!(claveAtrr is null) && !(campoAttr is null))
            {
               propClaves.Add(new ArregloClave() { Nombre = campoAttr.Nombre, Ordinal = claveAtrr.Ordinal });
            }
         }

         for (int k = 0; k < claves.Length; k++)
         {
            for (int m = 0; m < propClaves.Count; m++)
            {
               if (propClaves[m].Ordinal == k)
               {
                  propClaves[m].Valor = claves[k];
               }
            }
         }

         bool usarPx = configuracion.Enlace.Tipo == FuenteTipo.SqlServer;
         var sb = new txt.StringBuilder();
         sb.Append($"select * from {(usarPx ? modelo.Prefijo + "." : string.Empty)}{modelo.Nombre}");

         var pams = new lst.List<FuenteParametro>();

         for (int m = 0; m < propClaves.Count; m++)
         {
            if (m == 0)
               sb.Append(" where ");
            else
               sb.Append(" and ");

            sb.Append($"{propClaves[m].Nombre}={(usarPx ? "@" + propClaves[m].Nombre : "?")}");
            pams.Add(new FuenteParametro("@" + propClaves[m].Nombre, propClaves[m].Valor));
         }

         var comando = new FuenteSqlServer(configuracion.Enlace.Conexion)
         {
            Solicitud = sb.ToString(),
            Parametros = pams
         };
         Console.WriteLine(comando.Solicitud);
         var res = comando.Consultar();

         var instancia = Componente.Instanciar<T>(res.Fila0);

         return instancia;
      }
   }

   public class ArregloClave
   {
      public string Nombre { get; set; }
      public int Ordinal { get; set; }
      public object Valor { get; set; }
   }

   public class FuenteConfiguracion
   {
      public bool NombrarColumnas { get; set; }
      public FuenteEnlace Enlace { get; set; }
   }
}
