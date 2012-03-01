namespace ConsoleTests
{
   using System;
   using System.Text;
   using System.Collections.Generic;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   using OperationsResearch.Math;
   using OperationsResearch.Simplex;

   [TestClass]
   public class SimplexAlgorithmTests
   {
      [TestMethod]
      public void ConstructionTest()
      {
         SimplexAlgorithm subject;

         subject = new SimplexAlgorithm();

         Assert.AreEqual<Int32>(0, subject.Count);
         Assert.AreEqual<SimplexMode>(SimplexMode.Maximize, subject.Mode);
      }

      [TestMethod]
      public void FunctionalTest()
      {
         Rational[,] actual;
         String[] columnHeaders;
         Rational[,] expected;
         String[] rowHeaders;
         SimplexAlgorithm subject;
         Matrix<Rational> tableau;
         Rational[,] values;

         columnHeaders = new String[] { "x1", "x2", "s1", "s2", "s3", "s4", "g(0)" };
         expected = new Rational[,] { {12, 5, 0, 10, 24, 0, Rational.Undefined, 123 }};
         rowHeaders = new String[] { "I", "II", "III", "IV", "g" };
         values = new Rational[,]
         {
            { 1, 0, 1, 0, 0, 0, 0, 12 }, 
            { 0, 1, 0, 1, 0, 0, 0, 15 },
            { 1, 3, 0, 0, 1, 0, 0, 51 },
            { 3, 1, 0, 0, 0, 1, 0, 41 },
            {-9,-3, 0, 0, 0, 0, 1, 0 }
         };

         tableau = values;
         tableau.ColumnHeaders = columnHeaders;
         tableau.RowHeaders = rowHeaders;

         subject = new SimplexAlgorithm(tableau);
         subject.Run();
         actual = subject.Solutions[1].ToArray();

         Assert.AreEqual<Int32>(expected.GetLength(0), actual.GetLength(0));
         Assert.AreEqual<Int32>(expected.GetLength(1), actual.GetLength(1));
         for (Int32 m = 0; m < expected.GetLength(0); m++)
         {
            for (Int32 n = 0; n < expected.GetLength(1); n++)
            {
               Assert.AreEqual<Rational>(expected[m, n], actual[m, n]);
            }
         }
      }
   }
}
