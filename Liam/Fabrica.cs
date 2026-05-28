using System;
using System.Linq;
using cnc = System.Collections.Concurrent;
using lst = System.Collections.Generic;
using thr = System.Threading;

namespace Liam
{
   /// <summary>
   /// Representa una fábrica de objetos para la resolución de dependencias, permitiendo registrar tipos y estructuras de valores.
   /// </summary>
   public class Fabrica
      : IDisposable
   {
      // los tipos y estructuras de valores se almacenan usando diccionarios concurrentes para garantizar seguridad en ambientes multihilo.
      private readonly cnc.ConcurrentDictionary<Type, Func<Fabrica, object>> _registrados
          = new cnc.ConcurrentDictionary<Type, Func<Fabrica, object>>();

      // Los Singletons se almacenan como Lazy<object> para asegurar una sola inicialización en ambientes multihilo.
      private readonly cnc.ConcurrentDictionary<Type, Lazy<object>> _singletons
          = new cnc.ConcurrentDictionary<Type, Lazy<object>>();

      // Instancias de datos primitivos. Se recomienda no usar hasta mejorar el diseño para inyectar valores con nombre.
      //private readonly cnc.ConcurrentDictionary<Type, object> _valoresPrimitivos
      //    = new cnc.ConcurrentDictionary<Type, object>();

      // Secuencias de resolución por hilo usado para detectar dependencias circulares.
      private readonly thr.ThreadLocal<lst.HashSet<Type>> _resolviendo =
          new thr.ThreadLocal<lst.HashSet<Type>>(() => new lst.HashSet<Type>());

      public void Registrar<TContrato, TServicio>()
          where TServicio : TContrato
      {
         var serviceType = typeof(TContrato);
         var implementationType = typeof(TServicio);

         _registrados[serviceType] = (f) => { return f.Crear(implementationType); };
      }

      public void RegistrarUnico<TContrato, TServicio>()
          where TServicio : TContrato
      {
         var serviceType = typeof(TContrato);
         var implementationType = typeof(TServicio);

         _registrados[serviceType] = f =>
         {
            var lazy = _singletons.GetOrAdd(serviceType, t => new Lazy<object>(() => { return f.Crear(implementationType); }, true));

            return lazy.Value;
         };
      }

      public void Registrar<TServicio>(Func<Fabrica, object> factory)
      {
         if (factory is null)
            throw new ArgumentNullException(nameof(factory));

         var serviceType = typeof(TServicio);
         _registrados[serviceType] = factory;
      }

      public void RegistrarUnico<TServicio>(Func<Fabrica, object> factory)
      {
         if (factory is null)
            throw new ArgumentNullException(nameof(factory));

         var serviceType = typeof(TServicio);
         _registrados[serviceType] = f =>
         {
            var lazy = _singletons.GetOrAdd(serviceType, t => new Lazy<object>(() => factory(f), true));

            return lazy.Value;
         };
      }

      //public void RegistrarValor<T>(T valor)
      //{
      //   _valoresPrimitivos[typeof(T)] = valor;
      //}

      public T Crear<T>()
      {
         return (T)Crear(typeof(T));
      }

      private object Crear(Type tipo)
      {
         if (TryCrear(tipo, out object instance))
            return instance;

         throw new InvalidOperationException($"Could not resolve type '{tipo.FullName}'. Ensure it is registered or has a public constructor with resolvable parameters.");
      }

      // Try to create a type without throwing for unresolvable primitive parameters.
      // Throws for circular dependencies.
      private bool TryCrear(Type tipo, out object instance)
      {
         if (tipo == null)
            throw new ArgumentNullException(nameof(tipo));

         var resolving = _resolviendo.Value;
         if (resolving.Contains(tipo))
            throw new InvalidOperationException($"Circular dependency detected: {tipo.FullName}");

         resolving.Add(tipo);
         try
         {
            // exact primitive / provided value
            //if (_valoresPrimitivos.TryGetValue(tipo, out object provided))
            //{
            //   instance = provided;
            //   return true;
            //}

            // registered factory
            if (_registrados.TryGetValue(tipo, out Func<Fabrica, object> factory))
            {
               instance = factory(this);
               return true;
            }

            // try to find a registered implementation assignable to requested interface/base
            if (tipo.IsInterface || tipo.IsAbstract)
            {
               var impl = _registrados.Keys.FirstOrDefault(t => tipo.IsAssignableFrom(t));
               if (impl != null)
                  return TryCrear(impl, out instance);
            }

            // must be a concrete type now
            if (tipo.IsAbstract || tipo.IsInterface)
            {
               instance = null;
               return false;
            }

            var ctors = tipo.GetConstructors().OrderByDescending(c => c.GetParameters().Length);
            foreach (var ctor in ctors)
            {
               var parameters = ctor.GetParameters();
               var args = new object[parameters.Length];
               var ok = true;

               for (int i = 0; i < parameters.Length; i++)
               {
                  var p = parameters[i];
                  if (!TryCrear(p.ParameterType, out object resolved))
                  {
                     ok = false;
                     break;
                  }

                  args[i] = resolved;

                  // treat primitives, strings and value types as "primitive" parameters
                  //if (p.ParameterType.IsPrimitive || p.ParameterType == typeof(string) || p.ParameterType.IsValueType)
                  //{
                  //   if (!_valoresPrimitivos.TryGetValue(p.ParameterType, out object val))
                  //   {
                  //      ok = false;
                  //      break;
                  //   }
                  //   args[i] = val;
                  //}
                  //else
                  //{
                  //   if (!TryCrear(p.ParameterType, out object resolved))
                  //   {
                  //      ok = false;
                  //      break;
                  //   }
                  //   args[i] = resolved;
                  //}
               }

               if (!ok)
                  continue;

               instance = Activator.CreateInstance(tipo, args);
               return true;
            }

            instance = null;
            return false;
         }
         finally
         {
            resolving.Remove(tipo);
         }
      }

      public bool TryCrear<T>(out T instance)
      {
         if (TryCrear(typeof(T), out object obj) && obj is T t)
         {
            instance = t;
            return true;
         }

         instance = default;
         return false;
      }

      public void Dispose()
      {
         foreach (var kvp in _singletons)
         {
            var lazy = kvp.Value;
            if (lazy.IsValueCreated)
            {
               if (lazy.Value is IDisposable value)
               {
                  try { value.Dispose(); } catch { /* omitir */ }
               }
            }
         }

         _resolviendo.Dispose();
         _singletons.Clear();
         _registrados.Clear();
         //_valoresPrimitivos.Clear();
      }
   }
}
