using System;
using hdr = System.Net.Http.Headers;
using lst = System.Collections.Generic;
using sec = System.Net.Security;
using tsk = System.Threading.Tasks;
using txt = System.Text;
using w3n = System.Net.Http;
using xct = System.Security.Cryptography.X509Certificates;

namespace Liam
{
   /// <summary>
   /// Representa el cliente de servicio base para gestionar las llamadas a servicios.<br/>
   /// Gestiona los clientes de servicios de red en un solo punto de distribución.
   /// </summary>
   public class RedCliente
      : IDisposable
   {
      protected w3n.HttpClient ClienteHttp { get; private set; }
      protected bool AsegurarRespuestaCorrecta { get; set; } = true;

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="RedCliente"/>.
      /// </summary>
      public RedCliente()
      {
         this.ClienteHttp = new w3n.HttpClient();
         this.ClienteHttp.DefaultRequestHeaders.Accept.Clear();
         this.ClienteHttp.DefaultRequestHeaders.Accept.Add(new hdr.MediaTypeWithQualityHeaderValue("application/json"));
      }

      /// <summary>
      /// Asigna una autenticación a la autorización de la solicitud.
      /// </summary>
      /// <param name="simboloAtutenticacion"></param>
      public void AsignarAutorizacion(hdr.AuthenticationHeaderValue simboloAtutenticacion)
      {
         this.ClienteHttp.DefaultRequestHeaders.Authorization = simboloAtutenticacion;
      }

      /// <summary>
      /// Asigna una autenticación "Bearer Token" a la autorización de la solicitud.
      /// </summary>
      /// <param name="simbolo"></param>
      public void AsignarAutorizacionPortador(string simbolo)
      {
         this.AsignarAutorizacion(new hdr.AuthenticationHeaderValue("Bearer", simbolo));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<string>> LeerTexto(string puntoAcceso)
      {
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .GetAsync(puntoAcceso)
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<string>()
         {
            Contenido = await invocacion.Content.ReadAsStringAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<byte[]>> LeerBytes(string puntoAcceso)
      {
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .GetAsync(puntoAcceso)
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<byte[]>()
         {
            Contenido = await invocacion.Content.ReadAsByteArrayAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <param name="carga"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<string>> EnviarTexto(string puntoAcceso, string carga)
      {
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .PostAsync(puntoAcceso, new w3n.StringContent(carga, txt.Encoding.UTF8, "application/json"))
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<string>()
         {
            Contenido = await invocacion.Content.ReadAsStringAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <param name="carga"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<string>> EnviarTexto(string puntoAcceso, lst.Dictionary<string, string> carga)
      {
         var datos = new w3n.FormUrlEncodedContent(carga);
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .PostAsync(puntoAcceso, datos)
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<string>()
         {
            Contenido = await invocacion.Content.ReadAsStringAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <param name="carga"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<string>> ActualizarTexto(string puntoAcceso, string carga)
      {
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .PutAsync(puntoAcceso, new w3n.StringContent(carga))
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<string>()
         {
            Contenido = await invocacion.Content.ReadAsStringAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="puntoAcceso"></param>
      /// <returns></returns>
      protected async tsk.Task<RedRespuesta<string>> Eliminar(string puntoAcceso)
      {
         w3n.HttpResponseMessage invocacion = await this.ClienteHttp
            .DeleteAsync(puntoAcceso)
            .ConfigureAwait(false);

         if (this.AsegurarRespuestaCorrecta)
            invocacion.EnsureSuccessStatusCode();

         var respuesta = new RedRespuesta<string>()
         {
            Contenido = await invocacion.Content.ReadAsStringAsync(),
            Estado = (int)invocacion.StatusCode,
            EsCorrecto = invocacion.IsSuccessStatusCode
         };

         return respuesta;
      }

      /// <summary>
      /// Finalizar el cliente Http.
      /// </summary>
      public void Dispose()
      {
         this.ClienteHttp?.CancelPendingRequests();
         this.ClienteHttp?.Dispose();
      }

      /// <summary>
      /// Devuelve un indicador para omitir la validez de un certificado SSL.
      /// </summary>
      /// <param name="sender">Invocador del evento.</param>
      /// <param name="certificate">Certificado a evaluar.</param>
      /// <param name="chain">Encadenador de contenido de un certificado.</param>
      /// <param name="sslPolicyErrors">Enumeración con los errores del certificado.</param>
      /// <returns>Indicador de validez de un certificado SSL.</returns>
      public static bool AceptarCertificados(object sender, xct.X509Certificate certificate, xct.X509Chain chain, sec.SslPolicyErrors sslPolicyErrors)
      {
         string[] huellasPermitidas = new string[] {
            "HUELLAS_PERSONALIZADAS"
         };

         if (sslPolicyErrors != sec.SslPolicyErrors.None)
         {
            foreach (string huella in huellasPermitidas)
            {
               if (certificate.GetCertHashString() == huella)
               {
                  //certificate.Issuer,certificate.GetCertHashString();
                  return true;
               }
            }
            return false;
         }
         return true;
      }
   }
}
