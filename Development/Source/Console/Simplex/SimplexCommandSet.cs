namespace OperationsResearch.Simplex
{
   using System;
   using System.Collections.Generic;
   using System.Text;
   using System.Text.RegularExpressions;
   using System.Xml;

   using OperationsResearch.Math;

   [CommandSet("simplex")]
   internal sealed class SimplexCommandSet : ICommandSet
   {
      private class WriteTableauArguments
      {
         public WriteTableauArguments(Matrix<Rational> tableau, Int32 index)
         {
            this.Index = index;
            this.PivotColumn = -1;
            this.PivotRow = -1;
            this.Tableau = tableau;
         }

         public Int32 Index { get; private set; }

         public Int32 PivotColumn { get; set; }

         public Int32 PivotRow { get; set; }

         public Matrix<Rational> RatioTest { get; set; }

         public Matrix<Rational> Solution { get; set; }

         public Matrix<Rational> Tableau { get; private set; }

         public Matrix<RationalExpression> Transforms { get; set; }
      }

      public SimplexAlgorithm Algorithm { get; private set; }

      public SimplexArguments Parameters { get; private set; }

      public Int32 Execute(String[] arguments)
      {
         Matrix<Rational> tableau;

         this.Parameters = new SimplexArguments(arguments);

         // Load input and create initial tableau.
         tableau = Load();

         // Create and prepare algorithm.
         this.Algorithm = new SimplexAlgorithm();
         this.Algorithm.Mode = Parameters.Mode;
         this.Algorithm.Tableau = tableau;
         this.Algorithm.Run();

         // Save output.
         Save();

         // Signal success.
         return 0;
      }

      private Matrix<Rational> Load()
      {
         List<String> columnHeaders;
         List<List<String>> container;
         Int32 m;
         Int32 n;
         String[] lines;
         Matrix<Rational> result;
         Regex regex;
         List<String> rowHeaders;
         Char[] separator = new Char[] { ',' };

         columnHeaders = null;
         container = new List<List<String>>();
         m = 0;
         n = 0;
         regex = new Regex(@"^\s+$");
         rowHeaders = null;

         lines = System.IO.File.ReadAllLines(Parameters.Input);
         foreach (String line in lines)
         {
            List<String> values;

            if (String.IsNullOrEmpty(line) || line.StartsWith("#") || regex.IsMatch(line))
            {
               continue;
            }

            values = new List<string>(line.Split(separator, StringSplitOptions.None));

            n = System.Math.Max(n, values.Count);
            container.Add(values);
         }

         // Isolate column headers.
         if (Parameters.ColumnHeaders)
         {
            container.Remove(columnHeaders = container[0]);
            m = container.Count;
         }

         // Isolate row headers.
         if (Parameters.RowHeaders)
         {
            container.Remove(rowHeaders = container[0]);
            m = container.Count;
         }

         // Create matrix.
         result = new Matrix<Rational>(m, n);
         for (Int32 i = 0; i < container.Count; i++)
         {
            for (Int32 j = 0; j < container[i].Count; j++)
            {
               result[i, j] = Rational.Parse(container[i][j]);
            }
         }

         // Add column headers.
         if (null != columnHeaders)
         {
            result.ColumnHeaders = columnHeaders.ToArray();
         }

         // Add row headers.
         if (null != rowHeaders)
         {
            result.RowHeaders = rowHeaders.ToArray();
         }

         return result;
      }

      private void Save()
      {
         XmlWriterSettings settings;

         settings = new XmlWriterSettings();
         settings.Indent = true;

         using (XmlWriter writer = XmlWriter.Create(Parameters.Output, settings))
         {
            WriteTableauArguments arguments;
            Int32[] pivotColumns;
            Int32[] pivotRows;
            Matrix<Rational>[] ratioTests;
            Matrix<Rational>[] solutions;
            Matrix<RationalExpression>[] transforms;
            String xsi;

            pivotColumns = Algorithm.PivotColumns;
            pivotRows = Algorithm.PivotRows;
            ratioTests = Algorithm.RatioTests;
            solutions = Algorithm.Solutions;
            transforms = Algorithm.Transformations;
            xsi = "http://www.w3.org/2001/XMLSchema-instance";

            writer.WriteStartDocument();
            if (!String.IsNullOrEmpty(this.Parameters.Transform))
            {
               writer.WriteProcessingInstruction("xml-stylesheet", String.Format("type=\"text/xsl\" href=\"{0}\"", this.Parameters.Transform));
            }

            writer.WriteStartElement("OperationsResearch");

            writer.WriteStartAttribute("xsi", "schemaLocation", xsi);
            writer.WriteString("http://www.fom.de/schemas/operations-research/ws-11 Simplex-v1.0.xsd");
            writer.WriteEndAttribute(); // schemaLocation
            writer.WriteStartElement("Simplex");
            writer.WriteAttributeString("mode", Algorithm.Mode.ToString());
            writer.WriteStartElement("Tableaux");

            // Write initial tableau.
            arguments = new WriteTableauArguments(Algorithm.Tableau, -1);
            arguments.Solution = arguments.Tableau.GetRowVector(arguments.Tableau.M - 1);
            arguments.Solution[0, arguments.Solution.N - 2] = Rational.Undefined;
            if (0 != Algorithm.Count)
            {
               arguments.PivotColumn = Algorithm.PivotColumns[0];
               arguments.PivotRow = Algorithm.PivotRows[0];
               arguments.RatioTest = Algorithm.RatioTests[0];
               arguments.Transforms = Algorithm.Transformations[0];
            }

            WriteTableau(writer, arguments);

            // Write tableaux.
            for (Int32 index = 0; index < Algorithm.Count; index++)
            {
               arguments = new WriteTableauArguments(Algorithm[index], index);
               arguments.Solution = solutions[index];
               if (index < Algorithm.Count - 1)
               {
                  arguments.PivotColumn = pivotColumns[index + 1];
                  arguments.PivotRow = pivotRows[index + 1];
                  arguments.RatioTest = ratioTests[index + 1];
                  arguments.Transforms = transforms[index + 1];
               }

               WriteTableau(writer, arguments);
            }

            writer.WriteEndElement(); // Tableaux
            writer.WriteEndElement(); // Simplex
            writer.WriteEndElement(); // OperationsResearch
            writer.WriteEndDocument();

            writer.Close();
         }
      }

      private void WriteTableau(XmlWriter writer, WriteTableauArguments arguments)
      {
         writer.WriteStartElement("Tableau");
         writer.WriteAttributeString("index", (arguments.Index + 1).ToString());
         writer.WriteStartElement("Columns");
         for (Int32 n = 0; n < arguments.Tableau.N; n++)
         {
            writer.WriteStartElement("Column");
            writer.WriteAttributeString("n", (n + 1).ToString());
            writer.WriteString(arguments.Tableau.ColumnHeaders[n]);
            writer.WriteEndElement(); // Column
         }

         writer.WriteEndElement(); // Columns;

         // Pivot.
         writer.WriteStartElement("Pivot");
         writer.WriteAttributeString("m", (arguments.PivotRow + 1).ToString());
         writer.WriteAttributeString("n", (arguments.PivotColumn + 1).ToString());
         writer.WriteEndElement(); // Pivot

         // Rows.
         writer.WriteStartElement("Rows");
         for (Int32 m = 0; m < arguments.Tableau.M; m++)
         {
            writer.WriteStartElement("Row");
            writer.WriteAttributeString("m", (m + 1).ToString());
            writer.WriteAttributeString("function", arguments.Tableau.RowHeaders[m]);
            if (null != arguments.Transforms)
            {
               writer.WriteAttributeString("transform", arguments.Transforms[m, 0].ToString());
            }

            for (Int32 n = 0; n < arguments.Tableau.N; n++)
            {
               writer.WriteStartElement("Column");
               writer.WriteAttributeString("n", (n + 1).ToString());
               writer.WriteString(arguments.Tableau[m, n].ToString());
               writer.WriteEndElement(); // Column
            }

            writer.WriteStartElement("Ratio");
            if (null != arguments.RatioTest)
            {
               writer.WriteString(arguments.RatioTest[m, 0].ToString());
            }
            writer.WriteEndElement(); // Ratio
            writer.WriteEndElement(); // Row
         }

         writer.WriteEndElement(); // Rows

         // Solution.
         writer.WriteStartElement("Solution");
         if (null != arguments.Solution)
         {
            for (Int32 n = 0; n < arguments.Solution.N; n++)
            {
               writer.WriteStartElement("Column");
               writer.WriteAttributeString("n", (n + 1).ToString());
               writer.WriteString(arguments.Solution[0, n].ToString());
               writer.WriteEndElement(); // Column
            }
         }

         writer.WriteEndElement(); // Solution

         writer.WriteEndElement(); // Tableau
      }
   }
}
