using System;

namespace Liam
{
   /// <summary>
   /// Representa un valor lógico
   /// </summary>
   public class ValorLogico
   {
      private double Intensidad { get; set; }
      private TipoLogico Tipo { get; set; }

      private const double Maximo = 1.00d;
      private const double Minimo = 0.00d;

      private const double Umbral = 0.50d;
      private const double SinValor = -1.00d;

      private Func<object[], double> _metodo = null;
      private object[] _argumentos = null;
      private bool _invocado = false;

      /// <summary>
      /// Obtiene el valor Si (verdadero en lógica clásica, intensidad máxima en lógica difusa).
      /// </summary>
      public static ValorLogico Si { get; } = Maximo;

      /// <summary>
      /// Obtiene el valor No (falso en lógica clásica, intensidad mínima en lógica difusa).
      /// </summary>
      public static ValorLogico No { get; } = Minimo;

      /// <summary>
      /// Obtiene el valor indeterminado (instancia sin asignar).
      /// </summary>
      public static ValorLogico Indeterminado { get; } = new ValorLogico(SinValor, TipoLogico.Difuso);

      /// <summary>
      /// Almacena el texto para el valor predeterminado "Si".
      /// </summary>
      /// <remarks>Depende del idioma actual, asignado mediante el método <see cref="Supervisor.CambiarIdioma(string, object)"/></remarks>
      public static string SiTexto { get; set; } = "Si";

      /// <summary>
      /// Almacena el texto para el valor predeterminado "No".
      /// </summary>
      /// <remarks>Depende del idioma actual, asignado mediante el método <see cref="Supervisor.CambiarIdioma(string, object)"/></remarks>
      public static string NoTexto { get; set; } = "No";

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ValorLogico"/> con una intensidad.
      /// </summary>
      /// <param name="intensidad">Intensidad inicial del valor lógico.</param>
      /// <param name="tipo">Tipo de lógica del valor.</param>
      public ValorLogico(double intensidad, TipoLogico tipo = TipoLogico.Clasico)
      {
         if (intensidad > Maximo)
            intensidad = Maximo;
         else if (intensidad < Minimo && intensidad != SinValor)
            intensidad = Minimo;

         this.Intensidad = intensidad;
         this.Tipo = tipo;
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ValorLogico"/> con una referencia a método.
      /// </summary>
      /// <param name="funcion">Referencia a un método.</param>
      /// <param name="tipo">Tipo de lógica del valor.</param>
      public ValorLogico(Func<object[], double> funcion, TipoLogico tipo = TipoLogico.Clasico)
      {
         this.Intensidad = Minimo;
         this.Tipo = tipo;
         this._metodo = funcion;
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ValorLogico"/> con un valor de verdad clásico.
      /// </summary>
      public ValorLogico(bool valor)
         : this(valor ? Maximo : Minimo, TipoLogico.Clasico)
      {
      }

      /// <summary>
      /// Inicializa una nueva instancia de la clase <see cref="ValorLogico"/>.
      /// </summary>
      public ValorLogico()
         : this(false)
      {
      }

      private void Evaluar()
      {
         if (this._invocado || this._metodo is null)
            return;

         this.Intensidad = this._metodo.Invoke(this._argumentos);
         this._invocado = true;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="args"></param>
      /// <returns></returns>
      public ValorLogico Evaluar(object[] args)
      {
         if (this._metodo is null)
            throw new LiamException("TAREA: No se ha asignado un método para evaluar.");

         this._argumentos = args;
         this._invocado = false;
         this.Evaluar();

         return this;
      }

      /// <summary>
      /// Reasigna el método de evaluación.
      /// </summary>
      /// <param name="metodo">Referencia a un método.</param>
      public void ReasignarMetodo(Func<object[], double> metodo)
      {
         this._invocado = false;
         this._metodo = metodo;
      }

      /// <summary>
      /// Devuelve el valor lógico corto para la evaluación "Y".
      /// </summary>
      /// <param name="op">segundo operando.</param>
      /// <returns>Valor lógico corto para la evaluación "Y".</returns>
      public ValorLogico YTambien(ValorLogico op)
      {
         return new ValorLogico(this && op);
      }

      /// <summary>
      /// Devuelve el valor lógico corto para la evaluación "O".
      /// </summary>
      /// <param name="op">segundo operando.</param>
      /// <returns>Valor lógico corto para la evaluación "O".</returns>
      public ValorLogico OSino(ValorLogico op)
      {
         return new ValorLogico(this || op);
      }

      /// <summary>
      /// Devuelve el valor lógico para la evaluación "O exclusivo".
      /// </summary>
      /// <param name="op">segundo operando.</param>
      /// <returns>valor lógico para la evaluación "O exclusivo".</returns>
      public ValorLogico OBien(ValorLogico op)
      {
         return new ValorLogico((this || op) && !(this && op));
      }

      /// <summary>
      ///  Devuelve el valor lógico para la evaluación "Entonces".
      /// </summary>
      /// <param name="op">segundo operando.</param>
      /// <returns>Valor lógico para la evaluación "Entonces".</returns>
      public ValorLogico Entonces(ValorLogico op)
      {
         return new ValorLogico(!this || op);
      }

      /// <summary>
      /// Devuelve una cadena de texto con la intensidad en forma de número.
      /// </summary>
      /// <returns>Cadena de texto con la intensidad en forma de número.</returns>
      public override string ToString()
      {
         this.Evaluar();

         if (this.Tipo == TipoLogico.Clasico)
            return this ? SiTexto : NoTexto;
         else
            return $"{this.Intensidad:0.00000}";
      }

      /// <summary>
      /// Devuelve un valor luego de evaluar la expresion indicada. Se recomienda esté método para el encadenamiento de operaciones.
      /// </summary>
      /// <param name="expresion">Expresión a evaluar.</param>
      /// <returns>Valor luego de evaluar la expresion indicada.</returns>
      public static ValorLogico Determinar(ValorLogico expresion)
      {
         expresion?.Evaluar();
         return expresion ?? No;
      }

      /// <summary>
      /// Devuelve el valor de tipo <see cref="bool"/> del valor lógico.
      /// </summary>
      /// <param name="instancia">Instancia del valor lógico.</param>
      /// <returns></returns>
      public static implicit operator bool(ValorLogico instancia)
      {
         instancia?.Evaluar();

         return instancia?.Intensidad >= Umbral;
      }

      /// <summary>
      /// Devuelve el valor lógico a partir de un <see cref="bool"/>.
      /// </summary>
      /// <param name="valor">>Instancia de tipo <see cref="bool"/> a convertir.</param>
      /// <returns></returns>
      public static implicit operator ValorLogico(bool valor)
      {
         return new ValorLogico(valor);
      }

      /// <summary>
      /// Devuelve el valor de tipo <see cref="double"/> del valor lógico.
      /// </summary>
      /// <param name="instancia">Instancia del valor lógico.</param>
      /// <returns></returns>
      public static explicit operator double(ValorLogico instancia)
      {
         return instancia.Intensidad;
      }

      /// <summary>
      /// Devuelve el valor lógico a partir de un <see cref="double"/>.
      /// </summary>
      /// <param name="valor">Instancia de tipo <see cref="double"/> a convertir.</param>
      /// <returns></returns>
      public static implicit operator ValorLogico(double valor)
      {
         return new ValorLogico(valor);
      }

      /// <summary>
      /// Devuelve el valor de tipo <see cref="float"/> del valor lógico.
      /// </summary>
      /// <param name="instancia">Instancia del valor lógico.</param>
      /// <returns></returns>
      public static explicit operator float(ValorLogico instancia)
      {
         return (float)instancia.Intensidad;
      }

      /// <summary>
      /// Devuelve el valor lógico a partir de un <see cref="float"/>.
      /// </summary>
      /// <param name="valor">Instancia de tipo <see cref="float"/> a convertir.</param>
      /// <returns></returns>
      public static implicit operator ValorLogico(float valor)
      {
         return new ValorLogico(valor);
      }
      
      /// <summary>
      /// Devuelve el valor lógico opuesto del objeto.
      /// </summary>
      /// <param name="op">Objeto a oponer.</param>
      /// <returns>Valor lógico opuesto del objeto.</returns>
      public static ValorLogico operator !(ValorLogico op)
      {
         return new ValorLogico(Maximo - op.Intensidad);
      }

      /// <summary>
      /// Devuelve el valor lógico opuesto del objeto.
      /// </summary>
      /// <param name="op">Objeto a oponer.</param>
      /// <returns>Valor lógico opuesto del objeto.</returns>
      public static ValorLogico operator -(ValorLogico op)
      {
         return new ValorLogico(Maximo - op.Intensidad);
      }

      /// <summary>
      /// Devuelve la operación "O" cortocircuitada.
      /// </summary>
      /// <param name="op1">Primer operando.</param>
      /// <param name="op2">Segundo operando.</param>
      /// <returns>Operación "O" cortocircuitada.</returns>
      public static ValorLogico operator +(ValorLogico op1, ValorLogico op2)
      {
         return new ValorLogico(op1 || op2);
      }

      /// <summary>
      /// Devuelve la operación "Y" cortocircuitada.
      /// </summary>
      /// <param name="op1">Primer operando.</param>
      /// <param name="op2">Segundo operando.</param>
      /// <returns>Operación "Y" cortocircuitada.</returns>
      public static ValorLogico operator -(ValorLogico op1, ValorLogico op2)
      {
         return new ValorLogico(op1 || !op2);
      }

      /// <summary>
      /// Devuelve la operación de multiplicación.
      /// </summary>
      /// <param name="op1">Primer operando.</param>
      /// <param name="op2">Segundo operando.</param>
      /// <returns>Operación "Y" cortocircuitada.</returns>
      public static ValorLogico operator *(ValorLogico op1, ValorLogico op2)
      {
         return new ValorLogico(op1.Intensidad * op2.Intensidad, TipoLogico.Difuso);
      }
   }

   /// <summary>
   /// Identifica el tipo de contenido de un valor de verdad.
   /// </summary>
   public enum TipoLogico
   {
      /// <summary>
      /// Lógica clásica. Solo hay 2 valores: (si/no), (verdadero/falso), (1/0), etc.
      /// </summary>
      Clasico = 0,

      /// <summary>
      /// Lógica difusa. Cualquier decimal entre 0 y 1, ambos incluidos.
      /// </summary>
      Difuso = 1
   }
}
