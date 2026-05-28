using System;
using lst = System.Collections.Generic;

namespace Liam
{
   /// <summary>
   /// Representa el objeto base para interacción entre componentes del programa.
   /// </summary>
   [Serializable]
   public class Componente
      : IComponente
   {
      [NonSerialized]
      private readonly lst.Dictionary<string, object> misDatos;

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="Componente"/>.
      /// </summary>
      public Componente()
      {
         misDatos = new lst.Dictionary<string, object>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="nombre"></param>
      /// <returns></returns>
      public object Obtener(string nombre)
      {
         return misDatos.TryGetValue(nombre, out object valor) ? valor : null;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="nombre"></param>
      /// <param name="valor"></param>
      /// <returns></returns>
      public IComponente Editar(string nombre, object valor)
      {
         misDatos[nombre] = valor;
         return this;
      }
      /*
      protected internal void FilaMapear(FuenteFila datos)
      {
         misDatos.Clear();

         Type inst = this.GetType();
         var infoCmpnt = ComponenteRegistro.Obtener(inst);
         object valor;
         object valorPredef;
         int datoIndice;
         bool usarPredef;
         bool tienePredef;

         foreach (var propSel in infoCmpnt.Propiedades)
         {
            valor = null;
            valorPredef = null;
            usarPredef = false;
            tienePredef = false;

            datoIndice = datos.ObtenerIndice(propSel.CampoNombre);

            if (datoIndice >= 0)
            {
               valor = datos[datoIndice];
               MostrarSalida($" dc:= ", false);
            }
            else
            {
               MostrarSalida($" (no):= ", false);
            }

            if (propSel.ValorPredefinido != null)
            {
               tienePredef = true;
               valorPredef = propSel.ValorPredefinido.Valor;
            }

            if (valor is DBNull || valor is null)
            {
               valor = null;
               usarPredef = true;
            }
            else
            {
               if(propSel.Condiciones != null)
                  EvaluarCondiciones(propSel.Condiciones.Condiciones, ref valor, ref usarPredef);
            }

            if (usarPredef && tienePredef)
            {
               valor = valorPredef;
               MostrarSalida($"<df>", false);
            }

            MostrarSalida($"[{valor ?? "#null#"}] {valor?.GetType().Name ?? "-"}");

            if (!(valor is null))
            {
               propSel.ValorAsignar(this, valor);
            }
         }
      }*/

      /// <summary>
      /// 
      /// </summary>
      /// <param name="condiciones"></param>
      /// <param name="valor"></param>
      /// <param name="usarPredef"></param>
      protected internal void EvaluarCondiciones(DatoCondicion condiciones, ref object valor, ref bool usarPredef)
      {
         bool tieneConvertirANumero = false;

         if ((condiciones & DatoCondicion.ConvertirANumero32) == DatoCondicion.ConvertirANumero32)
         {
            valor = Convert.ChangeType(valor, typeof(int));
            tieneConvertirANumero = true;
         }
         else if ((condiciones & DatoCondicion.ConvertirANumero64) == DatoCondicion.ConvertirANumero64)
         {
            valor = Convert.ChangeType(valor, typeof(long));
            tieneConvertirANumero = true;
         }
         else if ((condiciones & DatoCondicion.ConvertirANumeroDec) == DatoCondicion.ConvertirANumeroDec)
         {
            valor = Convert.ChangeType(valor, typeof(decimal));
            tieneConvertirANumero = true;
         }
         if (tieneConvertirANumero)
         {
            if ((condiciones & DatoCondicion.NumeroCeroComoNulo) == DatoCondicion.NumeroCeroComoNulo)
            {
               if ((decimal)valor == 0m)
               {
                  valor = null;
                  usarPredef = true;
               }
            }
         }

         if ((condiciones & DatoCondicion.ConvertirATexto) == DatoCondicion.ConvertirATexto)
         {
            valor = Convert.ChangeType(valor, typeof(string));
         }

         if (valor is string auxValor)
         {
            // primero quitar espacios
            if ((condiciones & DatoCondicion.QuitarRelleno) == DatoCondicion.QuitarRelleno)
            {
               if (auxValor != null)
               {
                  auxValor = auxValor.Trim();
               }
            }
            // pasar a null si es cadena vacía.
            if ((condiciones & DatoCondicion.TextoVacioComoNulo) == DatoCondicion.TextoVacioComoNulo)
            {
               if (auxValor == string.Empty)
               {
                  auxValor = null;
                  usarPredef = true;
               }
            }
            // las demas operaciones (si hay cadena disponible)
            if (auxValor != null)
            {
               if ((condiciones & DatoCondicion.TransformarMinuscula) == DatoCondicion.TransformarMinuscula)
               {
                  auxValor = auxValor.ToLower();
               }
               if ((condiciones & DatoCondicion.TransformarMayuscula) == DatoCondicion.TransformarMayuscula)
               {
                  auxValor = auxValor.ToUpper();
               }
            }
            valor = auxValor;
         }
      }

      private static void MostrarSalida(string salida, bool finLinea = true)
      {
#if DEBUG_1
         if (Supervisor.Entorno == Entorno.Produccion)
            return;

         if (finLinea)
            System.Diagnostics.Debug.WriteLine(salida);
         else
            System.Diagnostics.Debug.Write(salida);
#endif
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{nameof(Componente)}";
      }
      /*
      /// <summary>
      /// Crea una nueva instancia de un objeto basado en <see cref="Componente"/>.
      /// </summary>
      /// <typeparam name="T">Clase derivada de <see cref="Componente"/>.</typeparam>
      /// <param name="datos">Datos para asignar a las propiedades del objeto.</param>
      public static T Instanciar<T>(object datos) where T : Componente, new()
      {
         var ret = new T();

         if (!(datos is null))
            ret.FilaMapear(fila);

         return ret;
      }*/
   }
}
