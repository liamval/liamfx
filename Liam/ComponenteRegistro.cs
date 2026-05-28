using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Liam
{
   public class ComponenteRegistro
   {
      private static readonly Dictionary<Type, ComponenteInformacion> _registrados = new Dictionary<Type, ComponenteInformacion>();

      public static ComponenteInformacion Obtener(Type tipo)
      {
         if (!_registrados.TryGetValue(tipo, out ComponenteInformacion info))
         {
            Registrar(tipo);
            info = _registrados[tipo];
         }
         return info;
      }

      public static ComponenteInformacion Obtener<TComponente>() where TComponente : Componente
         => Obtener(typeof(TComponente));

      public static void RegistrarEnsamblado(Assembly ensamblado)
      {
         if (ensamblado is null)
            throw new ArgumentNullException(nameof(ensamblado));

         var tipos = ensamblado.GetTypes()
            .Where(t =>
               !t.IsAbstract &&
               t.IsSubclassOf(typeof(Componente)) &&
               t.GetCustomAttribute<ModeloAttribute>() != null);

         foreach (var tipo in tipos)
         {
            Registrar(tipo);
         }
      }

      public static void Registrar(Type tipo)
      {
         if (tipo.GetCustomAttribute<ModeloAttribute>() is null)
            throw new InvalidOperationException($"{tipo.FullName} no tiene [Modelo]");

         var info = new ComponenteInformacion(tipo);
         if (info.Claves.Count == 0)
            throw new InvalidOperationException($"{tipo.FullName} tiene [Modelo] pero no [Clave].");

         _registrados[tipo] = info;
      }

      public static void Registrar<TComponente>() where TComponente : Componente
         => Registrar(typeof(TComponente));
   }
}
