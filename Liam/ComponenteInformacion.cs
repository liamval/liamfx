using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Liam
{
   public class ComponenteInformacion
   {
      public Type Tipo { get; }

      public string Tabla { get; }

      public string Esquema { get; }

      public string TablaCompleta => string.IsNullOrWhiteSpace(Esquema) ? Tabla : $"{Esquema}.{Tabla}";

      public List<PropiedadInformacion> Propiedades { get; }

      public List<PropiedadInformacion> Claves { get; }

      public List<PropiedadInformacion> Campos { get; }

      internal ComponenteInformacion(Type tipo)
      {
         var modelo = tipo.GetCustomAttribute<ModeloAttribute>();
         this.Tabla = modelo?.Nombre ?? tipo.Name;
         this.Esquema = modelo?.Prefijo;
         this.Tipo = tipo;

         this.Propiedades = new List<PropiedadInformacion>();
         var props = tipo.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
         foreach (var prop in props)
         {
            if (!prop.CanWrite)
               continue;

            var campo = prop.GetCustomAttribute<CampoAttribute>();
            if (campo is null)
               continue;

            var clave = prop.GetCustomAttribute<ClaveAttribute>();
            var valorPredefinido = prop.GetCustomAttribute<ValorPredefinidoAttribute>();
            var condiciones = prop.GetCustomAttribute<CondicionesAttribute>();

            var cp = new PropiedadInformacion(this, prop, campo, clave, valorPredefinido, condiciones);
            this.Propiedades.Add(cp);
         }

         this.Claves = Propiedades
             .Where(p => p.EsClave)
             .OrderBy(p => p.Clave.Ordinal)
             .ToList();

         this.Campos = Propiedades
             .Where(p => !p.EsClave)
             .ToList();
      }

      public override string ToString()
      {
         return this.TablaCompleta;
      }
   }
}
