namespace OperationsResearch
{
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Reflection;

   internal static class Program
   {
      internal static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
      internal static readonly string Version = Program.Assembly.GetName().Version.ToString();
      internal static readonly string Name = "Operations Research";
      internal static readonly string Title = String.Format("{0} (Version {1})", Program.Name, Program.Version);

      private static String[] Arguments { get; set; }
      private static Boolean NoLogo { get; set; }
      private static Boolean ShowUsageInfo { get; set; }
      private static Type CommandSet { get; set; }
      private static Dictionary<String, Type> CommandSets { get; set; }

      static Program()
      {
         Program.CommandSets = new Dictionary<string, Type>();

         foreach (Type type in Program.Assembly.GetTypes())
         {
            if (type.IsDefined(typeof(CommandSetAttribute), false))
            {
               CommandSetAttribute attribute = null;

               attribute = type.GetCustomAttributes(typeof(CommandSetAttribute), false)[0] as CommandSetAttribute;
               if ((null != attribute) && !Program.CommandSets.ContainsKey(attribute.Name.ToLowerInvariant()))
               {
                  Program.CommandSets.Add(attribute.Name.ToLowerInvariant(), type);
               }
            }
         }
      }

      /// <summary>
      /// Application entry point.
      /// </summary>
      internal static void Main()
      {
         // Return error unless tool succeeds.
         Environment.ExitCode = 1;

         // Parse command line arguments to determine settings and command set to use.
         Program.ParseArguments(Environment.GetCommandLineArgs());

         if (!Program.NoLogo)
         {
            Console.WriteLine(Program.Title);
            Console.WriteLine("Copyright © ??? 2011");
            Console.WriteLine();
         }

         if (Program.ShowUsageInfo)
         {
            if (null != Program.CommandSet)
            {
               string name = null;

               name = string.Format("OperationsResearch.{0}.usage", Program.CommandSet.Name);
               try
               {
                  using (StreamReader reader = new StreamReader(Program.Assembly.GetManifestResourceStream(name)))
                  {
                     Console.WriteLine(reader.ReadToEnd());
                  }
               }
               catch
               {
                  Console.WriteLine("Resource not found: {0}", name);
               }
            }
            else
            {
               foreach (string name in Program.Assembly.GetManifestResourceNames())
               {
                  if (name.EndsWith(".usage", StringComparison.OrdinalIgnoreCase))
                  {
                     using (StreamReader reader = new StreamReader(Program.Assembly.GetManifestResourceStream(name)))
                     {
                        Console.WriteLine(reader.ReadToEnd());
                     }
                  }
               }
            }

            return;
         }

         if (null != Program.CommandSet)
         {
            ICommandSet commandSet = null;

            commandSet = Activator.CreateInstance(Program.CommandSet) as ICommandSet;
            if (null == commandSet)
            {
               Console.WriteLine("{0} does not implement ICommandSet.", Program.CommandSet);
               return;
            }

            try
            {
               Environment.ExitCode = commandSet.Execute(Program.Arguments);
            }
            catch (CommandSetException ex)
            {
               Console.WriteLine("{0} failed.", Program.CommandSet);
               Console.WriteLine(ex);
            }
         }
      }

      private static void ParseArguments(String[] arguments)
      {
         List<String> remainingArguments = new List<String>();

         for (int i = 0; i < arguments.Length; i++)
         {
            String argument;
            
            argument = arguments[i].ToLowerInvariant();

            if (argument.Equals("/nologo", StringComparison.OrdinalIgnoreCase))
            {
               Program.NoLogo = true;
            }
            else if (argument.Equals("/?", StringComparison.OrdinalIgnoreCase))
            {
               Program.ShowUsageInfo = true;
            }
            else if (Program.CommandSets.ContainsKey(argument))
            {
               Program.CommandSet = Program.CommandSets[argument];
            }
            else
            {
               remainingArguments.Add(arguments[i]);
            }
         }

         Program.Arguments = remainingArguments.ToArray();
      }
   }
}
