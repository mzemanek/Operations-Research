namespace OperationsResearch.Simplex
{
   using System;
   using System.Collections.Generic;
   using System.Text;

   internal class SimplexArguments
   {
      public SimplexArguments(String[] arguments)
      {
         // Initialize properties.
         this.Mode = SimplexMode.Maximize;

         // Apply arguments.
         for (Int32 i = 0; i < arguments.Length; i++)
         {
            if (arguments[i].StartsWith("/") || arguments[i].StartsWith("-"))
            {
               switch (arguments[i].Substring(1).ToLowerInvariant())
               {
                  case "c":
                  case "columnheaders":
                     this.ColumnHeaders = true;
                     continue;

                  case "m":
                  case "mode":
                     this.Mode = (SimplexMode)Enum.Parse(typeof(SimplexMode), arguments[++i]);
                     continue;

                  case "o":
                  case "out":
                     this.Output = arguments[++i];
                     continue;

                  case "r":
                  case "rowheaders":
                     this.RowHeaders = true;
                     continue;

                  case "t":
                  case "transform":
                     this.Transform = arguments[++i];
                     continue;

                  default:
                     throw new NotSupportedException(String.Format("Invalid switch: {0}", arguments[i]));
               }
            }

            this.Input = arguments[i];
         }

         // Complement missing arguments.
         if (String.IsNullOrEmpty(Output) && !String.IsNullOrEmpty(Input))
         {
            Output = Input + ".xml";
         }

         // Validate arguments.
         if (String.IsNullOrEmpty(Input) || !System.IO.File.Exists(Input))
         {
            throw new CommandSetException("Invalid argument: input");
         }
      }

      public Boolean ColumnHeaders { get; set; }

      public String Input { get; set; }

      public SimplexMode Mode { get; set; }

      public String Output { get; set; }

      public Boolean RowHeaders { get; set; }

      public String Transform { get; set; }
   }
}
