namespace OperationsResearch.Math
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Text;
   using System.Text.RegularExpressions;

   public struct Rational // : IRational
   {
      private static readonly Regex Regex;

      public static readonly Rational Undefined = new Rational();
      public static readonly Rational One = new Rational(1, 1);
      public static readonly Rational Zero = new Rational(0, 1);

      private Int32 denominator;
      private Boolean isNegative;
      private Int32 numerator;

      static Rational()
      {
         RegexOptions options;
         StringBuilder pattern;

         options = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;

         pattern = new StringBuilder();
         pattern.Append(@"^(?<coefficient>0)$");
         pattern.Append(@"|");
         pattern.Append(@"^(?:\+|(?<negative>-))?");
         pattern.Append(@"(?:");
         pattern.Append(@"(?<coefficient>[1-9][0-9]*)");
         pattern.Append(@"|");
         pattern.Append(@"(?<coefficient>[1-9][0-9]*)??\s?");
         pattern.Append(@"(?<numerator>0|[1-9][0-9]*)/(?<denominator>[1-9][0-9]*)");
         pattern.Append(@")");
         pattern.Append(@"$");

         Regex = new Regex(pattern.ToString(), options);
      }

      public Rational(Int32 numerator, Int32 denominator)
      {
         if (0 == denominator)
         {
            throw new DivideByZeroException();
         }

         this.isNegative = ((0 > numerator) ^ (0 > denominator));
         this.numerator = Math.Abs(numerator);
         this.denominator = Math.Abs(denominator);
      }

      public Rational Add(Rational r)
      {
         Int32 lcd;
         Int32 numerator;

         lcd = Rational.LeastCommonMultiple(this.denominator, r.denominator);
         numerator = this.numerator * (lcd / this.denominator) * ((this.isNegative) ? -1 : 1);
         numerator += r.numerator * (lcd / r.denominator) * ((r.isNegative) ? -1 : 1);

         Rational.Reduce(ref numerator, ref lcd);

         return new Rational(numerator, lcd);
      }

      public Rational Divide(Rational r)
      {
         return this.Multiply(Rational.Reciprocal(r));
      }

      public Rational Multiply(Rational r)
      {
         Int32 denominator;
         Int32 numerator;

         denominator = this.denominator * r.denominator;
         numerator = this.numerator * r.numerator;

         Rational.Reduce(ref numerator, ref denominator);

         return new Rational(numerator, denominator) { isNegative = (this.isNegative ^ r.isNegative) };
      }

      public Rational Subtract(Rational r)
      {
         Int32 lcd;
         Int32 numerator;

         lcd = Rational.LeastCommonMultiple(this.denominator, r.denominator);
         numerator = this.numerator * (lcd / this.denominator) * ((this.isNegative) ? -1 : 1);
         numerator -= r.numerator * (lcd / r.denominator) * ((r.isNegative) ? -1 : 1);

         Rational.Reduce(ref numerator, ref lcd);

         return new Rational(numerator, lcd);
      }

      public override String ToString()
      {
         if (0 == this.denominator)
         {
            return "undefined";
         }
         else if (0 == numerator)
         {
            return "0";
         }
         else if (1 == denominator)
         {
            return String.Format("{0}{1}", (this.isNegative) ? "-" : "", this.numerator);
         }
         else
         {
            if (this.numerator > this.denominator)
            {
               Int32 mod = this.numerator % this.denominator;

               if (0 == mod)
               {
                  return String.Format("{0}{1}", (this.isNegative) ? "-" : "", (this.numerator / this.denominator));
               }
               else
               {
                  return String.Format("{0}{1} {2}/{3}", (this.isNegative) ? "-" : "", (this.numerator / this.denominator), mod, this.denominator);
               }
            }
            else
            {
               return String.Format("{0}{1}/{2}", (this.isNegative) ? "-" : "", this.numerator, this.denominator);
            }
         }
      }

      public static Boolean IsUnitVector(Rational[] vector)
      {
         Int32 ones;
         Int32 zeros;

         ones = 0;
         zeros = 0;

         for (Int32 i = 0; i < vector.Length; i++)
         {
            ones += Rational.One.Equals(vector[i]) ? 1 : 0;
            zeros += Rational.Zero.Equals(vector[i]) ? 1 : 0;
         }

         return ((vector.Length == ones + zeros) && (1 == ones));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      /// <exception cref="ArgumentNullException" />
      /// <exception cref="FormatException" />
      public static Rational Parse(String input)
      {
         Match match;

         // Validate input parameters.
         ThrowUtility.ThrowOnNullOrEmpty(input, "input");

         match = Regex.Match(input);
         if (match.Success)
         {
            checked
            {
               Int32 coefficient;
               Int32 denominator;
               Boolean negative;
               Int32 numerator;

               negative = match.Groups["negative"].Success;
               coefficient = (match.Groups["coefficient"].Success) ? Int32.Parse(match.Groups["coefficient"].Value) : 0;
               numerator = (match.Groups["numerator"].Success) ? Int32.Parse(match.Groups["numerator"].Value) : 0;
               denominator = (match.Groups["denominator"].Success) ? Int32.Parse(match.Groups["denominator"].Value) : 1;

               return new Rational((coefficient * denominator) + numerator, denominator) { isNegative = negative };
            }
         }

         throw new FormatException(String.Format("\"{0}\" is not a valid Rational expression.", input));
      }

      public static Rational Reciprocal(Rational r)
      {
         return new Rational(r.denominator, r.numerator) { isNegative = r.isNegative };
      }

      public static Rational Reduce(Rational r)
      {
         Int32 gcd;

         gcd = Rational.GreatestCommonDivisor(r.numerator, r.denominator);

         return new Rational(r.numerator / gcd, r.denominator / gcd) { isNegative = r.isNegative };
      }

      public static void Reduce(ref Int32 numerator, ref Int32 denominator)
      {
         Int32 gcd;

         gcd = Rational.GreatestCommonDivisor(numerator, denominator);
         numerator = numerator / gcd;
         denominator = denominator / gcd;
      }

      public static Boolean TryParse(String input, out Rational value)
      {
         value = Rational.Undefined;

         if (String.IsNullOrEmpty(input) || !Regex.IsMatch(input))
         {
            return false;
         }

         value = Parse(input);

         return true;
      }

      private static Int32 GreatestCommonDivisor(Int32 a, Int32 b)
      {
         return Rational.Euclidean(Math.Max(a, b), Math.Min(a, b));
      }

      private static Int32 LeastCommonMultiple(Int32 a, Int32 b)
      {
         // lcm(a,b) = |a*b|/gcd(a,b)
         return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
      }

      private static Int32 Euclidean(Int32 a, Int32 b)
      {
         // Euclidean algorithm.
         if (0 == b)
         {
            return a;
         }

         return Euclidean(b, a % b);
      }

      public static Rational operator +(Rational a, Rational b)
      {
         return a.Add(b);
      }

      public static Rational operator -(Rational a, Rational b)
      {
         return a.Subtract(b);
      }

      public static Rational operator *(Rational a, Rational b)
      {
         return a.Multiply(b);
      }

      public static Rational operator /(Rational a, Rational b)
      {
         return a.Divide(b);
      }

      public static implicit operator Double(Rational r)
      {
         return ((r.isNegative) ? -1.0 : 1.0) * (Convert.ToDouble(r.numerator) / Convert.ToDouble(r.denominator));
      }

      [Obsolete("Do not use, limited in functionality.", true)]
      public static implicit operator Rational(Double value)
      {
         Double factor;
         Int32 exponent;
         String s = null;

         s = value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
         exponent = s.Substring(s.IndexOf('.') + 1).Length;
         if (exponent > 9)
         {
            throw new OverflowException();
         }

         factor = Math.Pow(10.0, exponent);

         return new Rational((Int32)(value * factor), (Int32)factor);
      }

      public static implicit operator Rational(Int32 value)
      {
         return new Rational(value, 1);
      }

      public static implicit operator Rational(String input)
      {
         return Rational.Parse(input);
      }

      //RationalExpression IRational.AsExpression()
      //{
      //   return new RationalExpression(this);
      //}
   }
}
