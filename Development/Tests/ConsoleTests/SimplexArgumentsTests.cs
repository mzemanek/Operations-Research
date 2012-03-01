using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OperationsResearch;
using OperationsResearch.Simplex;

namespace ConsoleTests
{
   [TestClass]
   public class SimplexArgumentsTests
   {
      [ExpectedException(typeof(CommandSetException))]
      [TestMethod]
      public void ConstructionTest()
      {
         List<string> arguments;
         SimplexArguments subject;

         arguments = new List<string>();

         subject = new SimplexArguments(arguments.ToArray());
      }

      [TestMethod]
      public void ConstructionWithArguments()
      {
         List<string> arguments;
         string input;
         string output;
         SimplexMode mode;
         SimplexArguments subject;

         input = @".\Sample.csv";
         output = input + ".xml";
         mode = SimplexMode.Maximize;

         arguments = new List<string>();
         arguments.Add("/mode");
         arguments.Add(mode.ToString());
         arguments.Add("-c");
         arguments.Add(input);

         if (!System.IO.File.Exists(input))
         {
            System.IO.File.WriteAllText(input, "Test");
         }

         subject = new SimplexArguments(arguments.ToArray());

         Assert.AreEqual<string>(input, subject.Input);
         Assert.AreEqual<string>(output, subject.Output);
         Assert.AreEqual<SimplexMode>(mode, subject.Mode);
         Assert.IsTrue(subject.ColumnHeaders);
      }
   }
}
