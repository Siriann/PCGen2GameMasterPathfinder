using System;

namespace XMLCharacterConversionTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("\nUsage XMLCharacterConversionTool <isPC> <pathToFile>");
                Console.WriteLine("\t <isPC> can be true, or false otherwise");
                Console.WriteLine("\t <pathToFile> can be relative or absolute");

                Console.WriteLine("\nThe input file must be provided from PCGen and the export format has to be");
                Console.WriteLine("\tcsheet_fantasy_rpgwebprofiler.xml\n");
            }
            else
            {
                Character m_parser = new Character(args[1], args[0] == "true");
            }
            return;
        }
    }
}
