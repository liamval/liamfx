namespace Liam
{
   /// <summary>
   /// Representa el contexto de la estación donde se ejecuta el programa.
   /// </summary>
   public class EstacionInformacion
   {
      /// <summary>
      /// Obtiene el nombre de la estación.
      /// </summary>
      public string Nombre { get; internal set; }

      /// <summary>
      /// Obtiene la versión del CLR del motor .NET.
      /// </summary>
      public string MotorVersion { get; internal set; }

      /// <summary>
      /// Obtiene la descripción y versión del entorno de .NET.
      /// </summary>
      public string MotorDescripcion { get; internal set; }

      /// <summary>
      /// Obtiene la primera dirección IP v4 de la estación.
      /// </summary>
      public string DireccionIPv4 { get; internal set; }

      /// <summary>
      /// Obtiene la primera dirección IP v6 de la estación.
      /// </summary>
      public string DireccionIPv6 { get; internal set; }

      /// <summary>
      /// Obtiene el nombre de usuario del sistema operativo.
      /// </summary>
      public string PlataformaUsuario { get; internal set; }

      /// <summary>
      /// Obtiene el nombre del sistema operativo.
      /// </summary>
      public string PlataformaNombre { get; internal set; }

      /// <summary>
      /// Obtiene la versión del sistema operativo.
      /// </summary>
      public string PlataformaVersion { get; internal set; }

      /// <summary>
      /// Obtiene el indicador de 64bit de la plataforma.
      /// </summary>
      public bool PlataformaEs64Bit { get; internal set; }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="EstacionInformacion"/>.
      /// </summary>
      public EstacionInformacion()
      {
      }

      /// <summary>
      /// Devuelve una cadena de texto con la descripción de la instancia.
      /// </summary>
      /// <returns>Cadena de texto con la descripción de la instancia.</returns>
      public override string ToString()
      {
         return $"{this.Nombre}";
      }
   }
}