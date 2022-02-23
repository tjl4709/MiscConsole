using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiscConsole
{
    class MiscConsole
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("No program specified. Usage:\nprogram_name [program_options]");
            } else
                switch (args[0].ToLower()) {
                    case "wordle": new ParseWordle().Main(args); break;
                    default: Console.WriteLine("Unrecognized program name."); break;
                }
            Console.ReadKey();
        }
    }
}
