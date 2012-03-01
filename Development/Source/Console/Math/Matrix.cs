namespace OperationsResearch.Math
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Text;

   public class Matrix<T> : ICloneable
   {
      public delegate Boolean IsUnitVectorDelegate(T[] vector);

      private String[] columnHeaders;
      private readonly String name;
      private String[] rowHeaders;
      private T[,] values;

      //static Matrix()
      //{
      //   TypeConverter converter;

      //   converter = TypeDescriptor.GetConverter(typeof(T));
      //   One = (T)converter.ConvertFromString("1");
      //   Zero = (T)converter.ConvertFromString("0");
      //}

      public Matrix(Int32 m, Int32 n)
      {
         // Initialize members.
         this.columnHeaders = new String[n];
         this.name = String.Format("{0} X {1} {2}", m, n, typeof(T).Name);
         this.rowHeaders = new String[m];
         this.values = new T[m, n];
       
         // Initialize properties.
         this.M = m;
         this.N = n;

         Clear(this.columnHeaders);
         Clear(this.rowHeaders);
      }

      public Matrix(Int32 m, Int32 n, T value)
         : this(m, n)
      {
         for (UInt32 row = 0; row < this.M; row++)
         {
            for (UInt32 column = 0; column < this.N; column++)
            {
               this.values[row, column] = value;
            }
         }
      }

      public Matrix(Int32 m, Int32 n, T[] values)
         : this(m, n)
      {
         for (Int32 i = 0; i < values.Length; i++)
         {
            m = i / this.N;
            n = i - (m * this.N);
            this.values[m, n] = values[i];
         }
      }

      public T this[Int32 row, Int32 column]
      {
         [System.Diagnostics.DebuggerStepThrough]
         get
         {
            return this.values[row, column];
         }
         [System.Diagnostics.DebuggerStepThrough]
         set
         {
            this.values[row, column] = value;
         }
      }

      public string[] ColumnHeaders
      {
         [System.Diagnostics.DebuggerStepThrough]
         get { return this.columnHeaders; }
         set
         {
            Clear(this.columnHeaders);

            if (null != value)
            {
               for (Int32 i = 0; i < Math.Min(this.N, value.Length); i++)
               {
                  this.columnHeaders[i] = value[i];
               }
            }
         }
      }

      public Boolean IsColumnVector
      {
         [System.Diagnostics.DebuggerStepThrough]
         get { return (1 == this.N); }
      }

      public Boolean IsRowVector
      {
         [System.Diagnostics.DebuggerStepThrough]
         get { return (1 == this.M); }
      }

      public Boolean IsUnitVector
      {
         get
         {
            if (!SupportsUnitVector)
            {
               throw new NotSupportedException();
            }

            if (IsColumnVector)
            {
               T[] vector;

               vector = new T[this.M];
               for (Int32 i = 0; i < vector.Length; i++)
               {
                  vector[i] = this[i, 0];
               }

               return UnitVectorValidator(vector);
            }

            if (IsRowVector)
            {
               T[] vector;

               vector = new T[this.N];
               for (Int32 i = 0; i < vector.Length; i++)
               {
                  vector[i] = this[0, i];
               }

               return UnitVectorValidator(vector);
            }

            return false;
         }
      }

      public Int32 M { get; private set; }

      public Int32 N { get; private set; }

      //public Boolean IsUnitVector
      //{
      //   get
      //   {
      //      if (IsColumnVector || IsRowVector)
      //      {
      //         Int32 ones;
      //         Int32 zeros;

      //         ones = 0;
      //         zeros = 0;
      //         if (IsColumnVector)
      //         {
      //            for (UInt32 i = 0; i < this.m; i++)
      //            {
      //               ones += EqualityComparer<T>.Default.Equals(Matrix<T>.One, this.values[i, 0]) ? 1 : 0;
      //               zeros += EqualityComparer<T>.Default.Equals(Matrix<T>.Zero, this.values[i, 0]) ? 1 : 0;
      //            }

      //            return ((this.m == ones + zeros) && (1 == ones));
      //         }

      //         if (IsRowVector)
      //         {
      //            for (UInt32 i = 0; i < this.n; i++)
      //            {
      //               ones += EqualityComparer<T>.Default.Equals(Matrix<T>.One, this.values[0, i]) ? 1 : 0;
      //               zeros += EqualityComparer<T>.Default.Equals(Matrix<T>.Zero, this.values[0, i]) ? 1 : 0;
      //            }

      //            return ((this.n == ones + zeros) && (1 == ones));
      //         }
      //      }

      //      return false;
      //   }
      //}

      public String[] RowHeaders
      {
         [System.Diagnostics.DebuggerStepThrough]
         get { return this.rowHeaders; }
         set
         {
            Clear(this.rowHeaders);

            if (null != value)
            {
               for (Int32 i = 0; i < Math.Min(this.M, value.Length); i++)
               {
                  this.rowHeaders[i] = value[i];
               }
            }
         }
      }

      public Boolean SupportsUnitVector
      {
         [System.Diagnostics.DebuggerStepThrough]
         get { return (null != UnitVectorValidator); }
      }

      public IsUnitVectorDelegate UnitVectorValidator { get; set; }

      public Object Clone()
      {
         Matrix<T> result;

         result = new Matrix<T>(this.M, this.N);
         result.ColumnHeaders = this.ColumnHeaders;
         result.RowHeaders = this.RowHeaders;
         for (Int32 m = 0; m < this.M; m++)
         {
            for (Int32 n = 0; n < this.N; n++)
            {
               result[m, n] = this[m, n];
            }
         }

         return result;
      }

      public Int32 ComputeHashCode()
      {
         StringBuilder sb;

         sb = new StringBuilder();

         for (Int32 m = 0; m < this.M; m++)
         {
            for (Int32 n = 0; n < this.N; n++)
            {
               if (n != 0)
               {
                  sb.Append(" ");
               }

               sb.Append(this[m, n]);
            }

            sb.AppendLine();
         }

         return sb.ToString().GetHashCode();
      }

      public Matrix<T> GetColumnVector(Int32 index)
      {
         Matrix<T> result;

         result = new Matrix<T>(this.M, 1);
         result.UnitVectorValidator = this.UnitVectorValidator;
         for (Int32 i = 0; i < this.M; i++)
         {
            result[i, 0] = this[i, index];
         }
         
         return result;
      }

      public Matrix<T> GetRowVector(Int32 index)
      {
         Matrix<T> result;

         result = new Matrix<T>(1, this.N);
         result.UnitVectorValidator = this.UnitVectorValidator;
         for (Int32 i = 0; i < this.N; i++)
         {
            result[0, i] = this[index, i];
         }

         return result;
      }

      public static Matrix<T> Transpose(Matrix<T> matrix)
      {
         Matrix<T> result;

         result = new Matrix<T>(matrix.N, matrix.M);
         result.UnitVectorValidator = matrix.UnitVectorValidator;
         for (Int32 row = 0; row < matrix.M; row++)
         {
            for (Int32 column = 0; column < matrix.N; column++)
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

      public T[,] ToArray()
      {
         return this.values;
      }

      public override String ToString()
      {
         return this.name;
      }

      private static void Clear(String[] array)
      {
         if (null != array)
         {
            for (Int32 i = 0; i < array.Length; i++)
            {
               array[i] = String.Empty;
            }
         }
      }

      public static implicit operator Matrix<T>(T[,] input)
      {
         Int32 m;
         Int32 n;

         // Validate input parameters.
         ThrowUtility.ThrowOnNull(input, "input");

         m = input.GetLength(0);
         n = input.GetLength(1);

         return new Matrix<T>(m, n) { values = input };
      }
   }
}
