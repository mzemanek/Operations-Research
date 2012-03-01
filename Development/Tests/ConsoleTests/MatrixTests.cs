namespace ConsoleTests
{
   using System;
   using System.Text;
   using System.Collections.Generic;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   using OperationsResearch.Math;

   /// <summary>
   /// Summary description for MatrixTests
   /// </summary>
   [TestClass]
   public class MatrixTests
   {
      public MatrixTests()
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
      public void ConstructorTest()
      {
         Matrix<Int32> subject;

         subject = new Matrix<Int32>(1, 2, new Int32[] { 3, 4 });

         Assert.AreEqual<String>("1 X 2 Int32", subject.ToString());
         Assert.AreEqual<Int32>(1, subject.M);
         Assert.AreEqual<Int32>(2, subject.N);
         Assert.AreEqual<Int32>(3, subject[0, 0]);
         Assert.AreEqual<Int32>(4, subject[0, 1]);
      }

      [TestMethod]
      public void ImplicitOperatorArrayTest()
      {
         Matrix<Int32> subject;

         subject = new Int32[,] { { 1, 2, 3 }, { 4, 5, 6 } };

         Assert.AreEqual<String>("2 X 3 Int32", subject.ToString());
         Assert.AreEqual<Int32>(2, subject.M);
         Assert.AreEqual<Int32>(3, subject.N);
         Assert.AreEqual<Int32>(3, subject[0, 2]);
         Assert.AreEqual<Int32>(4, subject[1, 0]);
      }
   }
}
