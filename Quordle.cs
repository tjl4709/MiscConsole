using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiscConsole
{
    class Quordle
    {
        ParseWordle[] wordles;

        public void Main(string[] args)
        {
            wordles = new ParseWordle[4];
            for (int i = 0; i < wordles.Length; i++) {
                wordles[i] = new ParseWordle();
                wordles[i].MaxGuess = 9;
            }
            while (true) {
                Setup();
                for (int i = 0; i < 8; i++) {
                    bool completed = true;
                    for (int j = 0; j < wordles.Length; j++)
                        if (wordles[j].NumAnswers > 1) {
                            completed = false;
                            break;
                        }
                    if (completed) break;
                    WriteAnswers();
                    Console.Write($"Enter guess {i+1}: ");
                    Guess(Console.ReadLine().Split(' '));
                    Console.WriteLine();
                }
                WriteAnswers();
                if (!MiscConsole.Continue("Another game?"))
                    break;
                Console.WriteLine("\n");
            }
        }

        private void Setup() { for (int i = 0; i < wordles.Length; i++) wordles[i].Setup(); }
        private void WriteAnswers()
        {
            for (int i = 0; i < wordles.Length; i++) {
                Console.Write($"wordle #{i + 1}: ");
                if (wordles[i].NumAnswers > 1) {
                    Console.WriteLine(wordles[i].NumAnswers + " possible answers");
                    if (wordles[i].NumAnswers < 50)
                        Console.WriteLine(string.Join(" ", wordles[i].answers));
                } else if (wordles[i].NumAnswers == 1)
                    Console.WriteLine(wordles[i].answers[0]);
                else Console.WriteLine("No answer");
            }
        }

        private void Guess(string[] guess)
        {
            int i, offset = 1;
            for (i = 0; i < wordles.Length; i++)
                if (wordles[i].NumAnswers > 1)
                    offset++;
            if (guess.Length != offset) return;
            for (i = 0; i < guess.Length; i++)
                if (guess[i].Length != 5) return;
            offset = 1;
            for (i = 0; i < wordles.Length; i++)
                if (wordles[i].NumAnswers > 1)
                    wordles[i].Guess(new string[] { guess[0], guess[i + offset] });
                else offset--;
        }
    }
}
