namespace OperationsResearch.Simplex
{
   using System;
   using System.Collections.Generic;
   using System.Text;

   using OperationsResearch.Math;

   internal class SimplexAlgorithm
   {
      private List<Int32> hashCodes;
      private List<Int32> pivotColumns;
      private List<Int32> pivotRows;
      private List<Matrix<Rational>> ratioTests;
      private List<Matrix<Rational>> solutions;
      private Matrix<Rational> tableau;
      private List<Matrix<Rational>> tableaux;
      private List<Matrix<RationalExpression>> transformations;

      public SimplexAlgorithm()
      {
         this.hashCodes = new List<Int32>();
         this.pivotColumns = new List<Int32>();
         this.pivotRows = new List<Int32>();
         this.ratioTests = new List<Matrix<Rational>>();
         this.solutions = new List<Matrix<Rational>>();
         this.tableaux = new List<Matrix<Rational>>();
         this.transformations = new List<Matrix<RationalExpression>>();
      }

      public SimplexAlgorithm(Matrix<Rational> tableau)
         : this()
      {
         this.tableau = tableau;
      }

      public Matrix<Rational> this[Int32 index]
      {
         get { return this.tableaux[index]; }
      }

      public int Count
      {
         get { return this.tableaux.Count; }
      }

      public SimplexMode Mode { get; set; }

      public Int32[] PivotColumns
      {
         get { return this.pivotColumns.ToArray(); }
      }

      public Int32[] PivotRows
      {
         get { return this.pivotRows.ToArray(); }
      }

      public Matrix<Rational>[] RatioTests
      {
         get { return this.ratioTests.ToArray(); }
      }

      public Matrix<Rational>[] Solutions
      {
         get { return this.solutions.ToArray(); }
      }

      public Matrix<Rational> Tableau
      {
         get { return this.tableau; }
         set
         {
            this.tableau = value;
            this.Clear();
         }
      }

      public Matrix<Rational>[] Tableaux
      {
         get { return this.tableaux.ToArray(); }
      }

      public Matrix<RationalExpression>[] Transformations
      {
         get { return this.transformations.ToArray(); }
      }

      public void Clear()
      {
         this.hashCodes.Clear();
         this.pivotColumns.Clear();
         this.pivotRows.Clear();
         this.ratioTests.Clear();
         this.solutions.Clear();
         this.tableaux.Clear();
         this.transformations.Clear();
      }

      public void Run()
      {
         Matrix<Rational> matrix;
         Int32 pivotColumn;
         Int32 pivotRow;
         Matrix<Rational> ratioTest;

         this.Clear();
         matrix = (Matrix<Rational>)tableau.Clone();
         while (DeterminePivot(matrix, out pivotColumn, out pivotRow, out ratioTest))
         {
            Boolean abort;
            Matrix<RationalExpression> expressions;
            Int32 hashCode;

            // Determine transformations.
            expressions = new Matrix<RationalExpression>(matrix.M, 1);
            expressions[pivotRow, 0] = new RationalExpression(ArithmeticOperation.Divide, RationalExpression.Unknown, matrix[pivotRow, pivotColumn]);
            for (Int32 i = 0; i < matrix.M; i++)
            {
               if (i != pivotRow)
               {
                  RationalExpression right;

                  right = new RationalExpression(ArithmeticOperation.Multiply, matrix[i, pivotColumn], (RationalExpression)expressions[pivotRow, 0].Clone());
                  expressions[i, 0] = new RationalExpression(ArithmeticOperation.Subtract, RationalExpression.Unknown, right);
               }
            }

            // Apply transformations.
            for (Int32 n = 0; n < matrix.N; n++)
            {
               for (Int32 m = 0; m < expressions.M; m++)
               {
                  if (m != pivotRow)
                  {
                     expressions[m, 0].Left = matrix[m, n];
                     expressions[m, 0].Right.Right.Left = matrix[pivotRow, n];

                     matrix[m, n] = expressions[m, 0].Evaluate();
                  }
               }

               expressions[pivotRow, 0].Left = matrix[pivotRow, n];
               matrix[pivotRow, n] = expressions[pivotRow, 0].Evaluate();
            }

            // Translate expressions.
            expressions[pivotRow, 0].Left = "AZ";
            for (Int32 i = 0; i < expressions.M; i++)
            {
               if (i != pivotRow)
               {
                  expressions[i, 0].Left = String.Format("Z{0}", i + 1);
                  //expressions[i, 0].Right.Right.Left = String.Format("Z{0}", pivotRow + 1); // RationalExpression.Unknown;
                  expressions[i, 0].Right.Right = String.Format("Z{0}", pivotRow + 1);
               }
            }

            // Store pivot and ratio test.
            this.pivotColumns.Add(pivotColumn);
            this.pivotRows.Add(pivotRow);
            this.ratioTests.Add(ratioTest);

            // Determine base solution for the matrix.
            this.solutions.Add(DetermineSolution(matrix));

            // Store matrix and expressions.
            this.tableaux.Add(matrix);
            this.transformations.Add(expressions);

            // Determine and store hash code.
            hashCode = matrix.ComputeHashCode();
            abort = this.hashCodes.Contains(hashCode);
            this.hashCodes.Add(hashCode);

            // Abort in case a loop has been detected (= hash code already existing).
            if (abort)
            {
               break;
            }

            // Prepare for next run.
            matrix = (Matrix<Rational>)matrix.Clone();
         }
      }

      private Boolean DeterminePivot(Matrix<Rational> matrix, out Int32 column, out Int32 row, out Matrix<Rational> ratioTest)
      {
         column = DeterminePivotColumn(matrix);
         row = DeterminePivotRow(matrix, column, out ratioTest);

         return ((-1 != column) && (-1 != row));
      }

      private Int32 DeterminePivotColumn(Matrix<Rational> matrix)
      {
         Int32 index;
         Matrix<Rational> vector;

         index = -1;
         vector = matrix.GetRowVector(matrix.M - 1);
         if (SimplexMode.Maximize == this.Mode)
         {
            for (Int32 i = 0; i < matrix.N - 1; i++)
            {
               if (vector[0, i] < 0)
               {
                  if ((-1 == index) || (vector[0, i] < vector[0, index]))
                  {
                     index = i;
                  }
               }
            }
         }
         else if (SimplexMode.Minimize == this.Mode)
         {
            for (Int32 i = 0; i < matrix.N - 1; i++)
            {
               if (vector[0, i] > 0)
               {
                  if ((-1 == index) || (vector[0, i] > vector[0, index]))
                  {
                     index = i;
                  }
               }
            }
         }

         return index;
      }

      private Int32 DeterminePivotRow(Matrix<Rational> matrix, Int32 column, out Matrix<Rational> ratioTest)
      {
         Int32 index;

         ratioTest = matrix.GetColumnVector(matrix.N - 1);
         ratioTest[ratioTest.M - 1, 0] = Rational.Undefined;

         index = -1;
         if (-1 != column)
         {
            // Calculate ratios.  
            for (Int32 i = 0; i < matrix.M - 1; i++)
            {
               if (matrix[i, column] > 0)
               {
                  ratioTest[i, 0] /= matrix[i, column];
                  if ((-1 == index) || (ratioTest[i, 0] < ratioTest[index, 0]))
                  {
                     index = i;
                  }
               }
               else
               {
                  ratioTest[i, 0] = 0;
               }
            }
         }

         return index;
      }

      private Matrix<Rational> DetermineSolution(Matrix<Rational> matrix)
      {
         Matrix<Rational> lastColumn;
         Matrix<Rational> result;

         result = new Matrix<Rational>(1, matrix.N);
         result.ColumnHeaders = matrix.ColumnHeaders;
         result.RowHeaders = new String[] { "u" };

         lastColumn = matrix.GetColumnVector(matrix.N - 1);
         for (Int32 n = 0; n < result.N; n++)
         {
            if (n == result.N - 1)
            {
               result[0, n] = lastColumn[lastColumn.M - 1, 0];
            }
            else if (n != result.N - 2)
            {
               Matrix<Rational> column;

               column = matrix.GetColumnVector(n);
               column.UnitVectorValidator = Rational.IsUnitVector;
               if (column.IsUnitVector)
               {
                  for (Int32 m = 0; m < column.M; m++)
                  {
                     if (Rational.One.Equals(column[m, 0]))
                     {
                        result[0, n] = lastColumn[m, 0];
                        break;
                     }
                  }
               }
               else
               {
                  result[0, n] = 0;
               }
            }
         }

         return result;
      }
   }
}
