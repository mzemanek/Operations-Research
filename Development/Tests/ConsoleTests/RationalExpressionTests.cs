namespace ConsoleTests
{
   using System;
   using System.Text;
   using System.Collections.Generic;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   using OperationsResearch.Math;

   [TestClass]
   public class RationalExpressionTests
   {
      [TestMethod]
      public void ComposingTest()
      {
         RationalExpression left;
         RationalExpression right;
         RationalExpression subject;

         left = new RationalExpression(ArithmeticOperation.Add, 1, 1);
         right = new RationalExpression(ArithmeticOperation.Subtract, 1, 1);

         subject = left * right;

         Assert.AreEqual<String>("1 + 1 * 1 - 1", subject.ToString());
      }

      [TestMethod]
      public void ConstructionWithExpressionTest()
      {
         RationalExpression subject;

         subject = new RationalExpression(ArithmeticOperation.Add, 1, 1);

         Assert.AreEqual<String>("1 + 1", subject.ToString());
         Assert.AreEqual<Rational>(2, subject.Evaluate());
      }

      [TestMethod]
      public void ConstructionWithUnknownTest()
      {
         RationalExpression subject;

         subject = new RationalExpression(ArithmeticOperation.Divide, RationalExpression.Unknown, "1/2");

         Assert.AreEqual<String>("? / 1/2", subject.ToString());
      }

      [TestMethod]
      public void ConstructionWithVariableTest()
      {
         RationalExpression subject;

         subject = new RationalExpression(ArithmeticOperation.Multiply, "x1", "1/2");

         //Assert.AreEqual<String>("x1 * 1/2", subject.ToString());
      }
   }
}
