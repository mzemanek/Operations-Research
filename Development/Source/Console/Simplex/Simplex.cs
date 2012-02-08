namespace OperationsResearch.Simplex
{
   using System;
   using System.Collections.Generic;
   using System.Text;

   [CommandSet("simplex")]
   internal sealed class Simplex : ICommandSet
   {
      #region ICommandSet Members

      public Int32 Execute(String[] arguments)
      {
         SimplexArguments parameters;

         parameters = new SimplexArguments(arguments);

         // TODO: Continue here.
         throw new NotImplementedException();
      }

      #endregion
   }
}
