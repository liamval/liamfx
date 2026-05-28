using System;

namespace Liam.Datos
{
   [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
   public class FuenteDatosAttribute
      : Attribute
   {
      public Type Conexion { get; }

      public Type Comando { get; }

      public Type Lector { get; }

      public FuenteDatosAttribute(Type cnx, Type cmd, Type lct)
      {
         this.Conexion = cnx;
         this.Comando = cmd;
         this.Lector = lct;
      }
   }
}
