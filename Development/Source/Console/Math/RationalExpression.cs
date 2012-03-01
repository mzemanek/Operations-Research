namespace OperationsResearch.Math
{
   using System;
   using System.Collections.Generic;
   using System.Text;

   internal sealed class RationalExpression : ICloneable//, IRational
   {
      public static readonly RationalExpression Unknown = new RationalExpression(Rational.Undefined);

      public RationalExpression(Rational value)
      {
         this.Value = value;
      }

      public RationalExpression(String variable)
      {
         this.Operation = ArithmeticOperation.Variable;
         this.Variable = variable;
      }

      public RationalExpression(ArithmeticOperation operation, RationalExpression left, RationalExpression right)
      {
         this.Operation = operation;
         this.Left = left;
         this.Right = right;
      }

      private RationalExpression()
      {
         // Intentionally left blank;
      }

      internal RationalExpression Left { get; set; }

      internal ArithmeticOperation Operation { get; set; }

      internal RationalExpression Right { get; set; }

      internal Rational Value { get; set; }

      internal String Variable { get; set; }

      public Object Clone()
      {
         RationalExpression result;

         result = new RationalExpression();
         result.Left = Clone(this.Left);
         result.Operation = this.Operation;
         result.Right = Clone(this.Right);
         result.Value = this.Value;
          
         return result;
      }

      private RationalExpression Clone(RationalExpression input)
      {
         if ((null == input) || (RationalExpression.Unknown == input))
         {
            return input;
         }
         else
         {
            return (RationalExpression)input.Clone();
         }
      }

      public Rational Evaluate()
      {
         switch (this.Operation)
         {
            case ArithmeticOperation.Add:
               return this.Left.Evaluate() + this.Right.Evaluate();

            case ArithmeticOperation.Divide:
               return this.Left.Evaluate() / this.Right.Evaluate();

            case ArithmeticOperation.Multiply:
               return this.Left.Evaluate() * this.Right.Evaluate();

            case ArithmeticOperation.None:
               return this.Value;

            case ArithmeticOperation.Subtract:
               return this.Left.Evaluate() - this.Right.Evaluate();

            default:
               return Rational.Undefined;
         }
      }

      public override string ToString()
      {
         if (this.Operation == ArithmeticOperation.None)
         {
            if (RationalExpression.Unknown == this)
            {
               return "?";
            }

            return this.Value.ToString();
         }
         else if (this.Operation == ArithmeticOperation.Variable)
         {
            return this.Variable;
         }
         else
         {
            Char op;

            switch (this.Operation)
            {
               case ArithmeticOperation.Add:
                  op = '+';
                  break;

               case ArithmeticOperation.Divide:
                  op = '/';
                  break;

               case ArithmeticOperation.Multiply:
                  op = '*';
                  break;

               case ArithmeticOperation.Subtract:
                  op = '-';
                  break;

               default:
                  op = '?';
                  break;
            }

            return String.Format("{0} {1} {2}", this.Left.ToString(), op, this.Right.ToString());
         }
      }

      public static RationalExpression operator +(RationalExpression a, RationalExpression b)
      {
         return new RationalExpression(ArithmeticOperation.Add, a, b);
      }

      public static RationalExpression operator -(RationalExpression a, RationalExpression b)
      {
         return new RationalExpression(ArithmeticOperation.Subtract, a, b);
      }

      public static RationalExpression operator *(RationalExpression a, RationalExpression b)
      {
         return new RationalExpression(ArithmeticOperation.Multiply, a, b);
      }

      public static RationalExpression operator /(RationalExpression a, RationalExpression b)
      {
         return new RationalExpression(ArithmeticOperation.Divide, a, b);
      }

      public static implicit operator RationalExpression(Int32 value)
      {
         return new RationalExpression((Rational)value);
      }

      public static implicit operator RationalExpression(Rational value)
      {
         return new RationalExpression(value);
      }

      public static implicit operator RationalExpression(String value)
      {
         Rational value2;

         if (Rational.TryParse(value, out value2))
         {
            return new RationalExpression(value2);
         }
         else
         {
            return new RationalExpression(value);
         }
      }

      //RationalExpression IRational.AsExpression()
      //{
      //   return this;
      //}
   }
}
