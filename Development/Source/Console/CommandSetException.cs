namespace OperationsResearch
{
   using System;
   using System.Runtime.Serialization;

   [Serializable]
   public class CommandSetException : Exception
   {
      public CommandSetException()
         : base()
      {
         // Intentionally left blank.
      }

      public CommandSetException(String message)
         : base(message)
      {
         // Intentionally left blank.
      }

      public CommandSetException(String message, Exception innerException)
         : base(message, innerException)
      {
         // Intentionally left blank.
      }

      protected CommandSetException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
         // Intentionally left blank.
      }
   }
}
