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
                Console.WriteLine("No program specified. Available programs: wordle quordle time");
                Console.Write("Choose one of the above programs: ");
                args = new string[] { Console.ReadLine() };
            }
            do {
                switch (args[0].ToLower()) {
                    case "wordle": new ParseWordle().Main(args); break;
                    case "quordle": new Quordle().Main(args); break;
                    case "time": new TimeCalculator().Main(args); break;
                    default: Console.WriteLine("Unrecognized program name."); break;
                }
                Console.Write("Would you like to run another program (wordle quordle time) or exit: ");
                args[0] = Console.ReadLine();
            } while (args[0] != "exit");
            Console.Write("Press any key to exit . . .");
            Console.ReadKey();
        }
        public static bool Continue(string msg)
        {
            Console.Write(msg + " (y/n): ");
            while (true) {
                string str = Console.ReadLine().Trim().ToLower();
                if (str == "yes" || str == "y")
                    return true;
                else if (str == "no" || str == "n")
                    return false;
                Console.Write("Unexpected answer. Try again (y/n): ");
            }
        }
    }
}
