namespace OperationsResearch
{
   using System;

   internal static class ThrowUtility
   {
      [System.Diagnostics.DebuggerNonUserCode]
      [System.Diagnostics.DebuggerStepThrough]
      public static void ThrowOnNull(Object value, String parameterName)
      {
         if (null == value)
         {
            throw new ArgumentNullException(parameterName);
         }
      }

      [System.Diagnostics.DebuggerNonUserCode]
      [System.Diagnostics.DebuggerStepThrough]
      public static void ThrowOnNullOrEmpty(String value, String parameterName)
      {
         if (String.IsNullOrEmpty(value))
         {
            throw new ArgumentNullException(parameterName);
         }
      }
   }
}
