using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

class DumpMef
{
    private static readonly HashSet<string> assemblyFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: DumpMef.exe <path-to-dll>.dll");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            return;
        }

        assemblyFolders.Add(Environment.CurrentDirectory);
        assemblyFolders.Add(Path.GetDirectoryName(filePath));

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        var catalog = new AssemblyCatalog(filePath);
        DumpCatalog(catalog);
    }

    private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyShortName = new AssemblyName(args.Name).Name;

        foreach (var folder in assemblyFolders)
        {
            var candidate = Path.Combine(folder, assemblyShortName + ".dll");
            if (File.Exists(candidate))
            {
                var name = AssemblyName.GetAssemblyName(candidate);
                return Assembly.Load(name);
            }
        }

        return null;
    }

    private static void DumpCatalog(AssemblyCatalog catalog)
    {
        foreach (var composablePartDefinition in catalog.OrderBy(s => s.ToString()))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(composablePartDefinition.ToString());

            if (composablePartDefinition.ExportDefinitions.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  Exports:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var export in composablePartDefinition.ExportDefinitions.OrderBy(e => e.ContractName))
                {
                    Console.WriteLine("    " + export.ContractName);
                }
            }

            if (composablePartDefinition.ImportDefinitions.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  Imports:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var import in composablePartDefinition.ImportDefinitions.OrderBy(e => e.ContractName))
                {
                    Console.WriteLine("    " + import.ContractName);
                }
            }
        }
    }
}