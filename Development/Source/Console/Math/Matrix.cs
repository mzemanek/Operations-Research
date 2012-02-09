namespace OperationsResearch.Math
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Text;

   public class Matrix<T>
   {
      private static readonly T One;
      private static readonly T Zero;

      private readonly UInt32 m;
      private readonly UInt32 n;
      private readonly String name;
      private T[,] values;

      static Matrix()
      {
         TypeConverter converter;

         converter = TypeDescriptor.GetConverter(typeof(T));
         One = (T)converter.ConvertFromString("1");
         Zero = (T)converter.ConvertFromString("0");
      }

      public Matrix(UInt32 m, UInt32 n)
      {
         this.m = m;
         this.n = n;
         this.name = String.Format("{0} X {1} {2}", m, n, typeof(T).Name);
         this.values = new T[m, n];
      }

      public Matrix(UInt32 m, UInt32 n, T value)
         : this(m, n)
      {
         for (UInt32 row = 0; row < this.m; row++)
         {
            for (UInt32 column = 0; column < this.n; column++)
            {
               this.values[row, column] = value;
            }
         }
      }

      public Matrix(UInt32 m, UInt32 n, T[] values)
         : this(m, n)
      {
         for (UInt32 i = 0; i < values.Length; i++)
         {
            m = i / this.n;
            n = i - (m * this.n);
            this.values[m, n] = values[i];
         }
      }

      public T this[UInt32 row, UInt32 column]
      {
         get
         {
            return this.values[row, column];
         }
         set
         {
            this.values[row, column] = value;
         }
      }

      public Boolean IsColumnVector
      {
         get { return (1 == this.n); }
      }

      public Boolean IsRowVector
      {
         get { return (1 == this.m); }
      }

      public Boolean IsUnitVector
      {
         get
         {
            if (IsColumnVector || IsRowVector)
            {
               Int32 ones;
               Int32 zeros;

               ones = 0;
               zeros = 0;
               if (IsColumnVector)
               {
                  for (UInt32 i = 0; i < this.m; i++)
                  {
                     ones += EqualityComparer<T>.Default.Equals(Matrix<T>.One, this.values[i, 0]) ? 1 : 0;
                     zeros += EqualityComparer<T>.Default.Equals(Matrix<T>.Zero, this.values[i, 0]) ? 1 : 0;
                  }

                  return ((this.m == ones + zeros) && (1 == ones));
               }

               if (IsRowVector)
               {
                  for (UInt32 i = 0; i < this.n; i++)
                  {
                     ones += EqualityComparer<T>.Default.Equals(Matrix<T>.One, this.values[0, i]) ? 1 : 0;
                     zeros += EqualityComparer<T>.Default.Equals(Matrix<T>.Zero, this.values[0, i]) ? 1 : 0;
                  }

                  return ((this.n == ones + zeros) && (1 == ones));
               }
            }

            return false;
         }
      }

      public Matrix<T> GetColumnVector(UInt32 index)
      {
         Matrix<T> result;

         result = new Matrix<T>(this.m, 1);
         for (UInt32 i = 0; i < this.m; i++)
         {
            result[i, 0] = this[i, index];
         }

         return result;
      }

      public Matrix<T> GetRowVector(UInt32 index)
      {
         Matrix<T> result;

         result = new Matrix<T>(1, this.n);
         for (UInt32 i = 0; i < this.n; i++)
         {
            result[0, i] = this[index, i];
         }

         return result;
      }

      public static Matrix<T> Transpose(Matrix<T> matrix)
      {
         Matrix<T> result;

         result = new Matrix<T>(matrix.n, matrix.m);
         for (UInt32 row = 0; row < matrix.m; row++)
         {
            for (UInt32 column = 0; column < matrix.n; column++)
            {
               result.values[column, row] = matrix.values[row, column];
            }
         }

         return result;
      }

      public Matrix<T> Transpose()
      {
         return Transpose(this);
      }

      public override String ToString()
      {
         return this.name;
      }
   }
}
