using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

class DumpMef
{
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

        var catalog = new AssemblyCatalog(filePath);
        DumpCatalog(catalog);
    }

    private static void DumpCatalog(AssemblyCatalog catalog)
    {
        foreach (var composablePartDefinition in catalog)
        {
            Console.WriteLine(composablePartDefinition.ToString());
            Console.WriteLine("  Exports:");
            foreach (var export in composablePartDefinition.ExportDefinitions.OrderBy(e => e.ContractName))
            {
                Console.WriteLine("    " + export.ContractName);
            }

            Console.WriteLine("  Imports:");
            foreach (var import in composablePartDefinition.ImportDefinitions.OrderBy(e => e.ContractName))
            {
                Console.WriteLine("    " + import.ContractName);
            }
        }
    }
}