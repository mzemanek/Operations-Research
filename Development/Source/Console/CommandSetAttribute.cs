namespace OperationsResearch
{
   using System;

   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
   internal sealed class CommandSetAttribute : Attribute
   {
      public CommandSetAttribute(String name)
         : base()
      {
         this.Name = name;
      }

      public String Name { get; set; }
   }
}
