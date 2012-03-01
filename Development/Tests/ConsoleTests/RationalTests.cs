namespace ConsoleTests
{
   using System;
   using System.Text;
   using System.Text.RegularExpressions;
   using System.Collections.Generic;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   using OperationsResearch.Math;

   /// <summary>
   /// Summary description for UnitTest1
   /// </summary>
   [TestClass]
   public class RationalTests
   {
      public RationalTests()
      {
         //
         // TODO: Add constructor logic here
         //
      }

      private TestContext testContextInstance;

      /// <summary>
      ///Gets or sets the test context which provides
      ///information about and functionality for the current test run.
      ///</summary>
      public TestContext TestContext
      {
         get
         {
            return testContextInstance;
         }
         set
         {
            testContextInstance = value;
         }
      }

      #region Additional test attributes
      //
      // You can use the following additional attributes as you write your tests:
      //
      // Use ClassInitialize to run code before running the first test in the class
      // [ClassInitialize()]
      // public static void MyClassInitialize(TestContext testContext) { }
      //
      // Use ClassCleanup to run code after all tests in a class have run
      // [ClassCleanup()]
      // public static void MyClassCleanup() { }
      //
      // Use TestInitialize to run code before running each test 
      // [TestInitialize()]
      // public void MyTestInitialize() { }
      //
      // Use TestCleanup to run code after each test has run
      // [TestCleanup()]
      // public void MyTestCleanup() { }
      //
      #endregion

      [TestMethod]
      public void AddTest()
      {
         Rational subject;

         subject = "1/2";

         subject += "25 1/2";

         Assert.AreEqual<Rational>(26, subject);
      }

      [TestMethod]
      public void DivideTest()
      {
         Rational subject;

         subject = "1/2";

         subject /= "1/2";

         Assert.AreEqual<Rational>(Rational.One, subject);
      }

      [TestMethod]
      public void ImplicitOperatorDoubleTest()
      {
         Rational subject;

         subject = "1/4";

         Assert.AreEqual<Double>(0.25, subject);
      }

      [TestMethod]
      public void ImplicitOperatorStringTest()
      {
         Rational subject;

         subject = "1";

         Assert.AreEqual<Double>(1.0, subject);
      }

      [ExpectedException(typeof(FormatException))]
      [TestMethod]
      public void ImplicitOperatorStringFailTest()
      {
         Rational subject;

         subject = "x1";
      }

      [TestMethod]
      public void IsUnitVectorTest()
      {
         Rational[] expectedFailure;
         Rational[] expectedSuccess;

         expectedFailure = new Rational[] { 1, 2, 3, 4 };
         expectedSuccess = new Rational[] { 0, 1, 0, 0 };

         Assert.IsFalse(Rational.IsUnitVector(expectedFailure));
         Assert.IsTrue(Rational.IsUnitVector(expectedSuccess));
      }

      [TestMethod]
      public void MultiplyTest()
      {
         Rational subject;

         subject = "1/2";

         subject *= "1/2";

         Assert.AreEqual<Rational>("1/4", subject);
      }

      [ExpectedException(typeof(FormatException))]
      [TestMethod]
      public void ParseInvalidStringTest()
      {
         Rational subject;
         String value;

         value = "la le lu";
       
         subject = Rational.Parse(value);
      }

      [ExpectedException(typeof(ArgumentNullException))]
      [TestMethod]
      public void ParseNullTest()
      {
         Rational subject;
         String value;

         value = null;

         subject = Rational.Parse(value);
      }

      [TestMethod]
      public void ParseValidStringTest()
      {
         Rational subject;
         String value;

         // Arrange.
         value = "-1 1/3";

         // Act.
         subject = Rational.Parse(value);

         // Assert.
         Assert.AreEqual<String>(value, subject.ToString());
      }

      [TestMethod]
      public void ReciprocalTest()
      {
         Assert.AreEqual<Rational>("8/4", Rational.Reciprocal("4/8"));
      }

      [TestMethod]
      public void ReduceTest()
      {
         Assert.AreEqual<Rational>("1/2", Rational.Reduce("3/6"));
      }

      [TestMethod]
      public void SubtractTest()
      {
         Rational subject;

         subject = "3/4";

         subject -= "1/2";

         Assert.AreEqual<Rational>("1/4", subject);
      }
   }
}
