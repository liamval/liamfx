using System;
using lex = System.Linq.Expressions;
using rfl = System.Reflection;

namespace Liam
{
   public class PropiedadInformacion
   {
      public rfl.PropertyInfo Propiedad { get; }

      public Type Tipo { get; }

      public CampoAttribute Campo { get; }

      public ClaveAttribute Clave { get; }

      public ValorPredefinidoAttribute ValorPredefinido { get; }

      public CondicionesAttribute Condiciones { get; }

      private ComponenteInformacion Componente { get; }

      public bool EsClave => Clave != null;

      public string CampoNombre => Campo.Nombre;

      public string ParametroNombre => $"@{Campo.Nombre}";

      public Action<object, object> ValorAsignar;

      public Func<object, object> ValorObtener;

      public PropiedadInformacion(ComponenteInformacion componenteInfo, rfl.PropertyInfo propiedad, CampoAttribute campo, ClaveAttribute clave, ValorPredefinidoAttribute valorPredefinido, CondicionesAttribute condiciones)
      {
         Propiedad = propiedad;
         Campo = campo;
         Clave = clave;
         ValorPredefinido = valorPredefinido;
         Condiciones = condiciones;
         Componente = componenteInfo;

         var tipoDeclarado = propiedad.PropertyType;
         Tipo = Nullable.GetUnderlyingType(tipoDeclarado) ?? tipoDeclarado;
         this.ValorObtener = ValorObtenerExpresion(propiedad, Componente.Tipo);
         this.ValorAsignar = ValorAsignarExpresion(propiedad, Componente.Tipo);
      }

      public override string ToString()
      {
         return $"{this.Propiedad.Name} ({this.CampoNombre})";
      }

      public static Func<object, object> ValorObtenerExpresion(rfl.PropertyInfo propiedad, Type tipoEntidad)
      {
         var obj = lex.Expression.Parameter(typeof(object), "obj");
         var prop = lex.Expression.Property(lex.Expression.Convert(obj, tipoEntidad), propiedad);
         var cuerpo = lex.Expression.Convert(prop, typeof(object));

         return lex.Expression
             .Lambda<Func<object, object>>(cuerpo, obj)
             .Compile();
      }

      public static Action<object, object> ValorAsignarExpresion(rfl.PropertyInfo propiedad, Type tipoEntidad)
      {
         var obj = lex.Expression.Parameter(typeof(object), "obj");
         var valor = lex.Expression.Parameter(typeof(object), "valor");
         var prop = lex.Expression.Property(lex.Expression.Convert(obj, tipoEntidad), propiedad);
         var conv = lex.Expression.Convert(valor, propiedad.PropertyType);
         var cuerpo = lex.Expression.Assign(prop, conv);

         return lex.Expression
             .Lambda<Action<object, object>>(cuerpo, obj, valor)
             .Compile();
      }
   }
}
