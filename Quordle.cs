using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiscConsole
{
    class Quordle
    {
        ParseWordle[] wordles;
        string exclude;
        List<string> guesses;

        public void Main(string[] args)
        {
            exclude = "";
            guesses = new List<string>();
            guesses.AddRange(File.ReadLines("..\\..\\guesses.txt"));
            foreach (string word in File.ReadLines("..\\..\\wordle.txt"))
                if (!guesses.Contains(word)) guesses.Add(word);
            wordles = new ParseWordle[4];
            for (int i = 0; i < wordles.Length; i++) {
                wordles[i] = new ParseWordle(guesses);
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
                    Suggest();
                    Console.Write($"Enter guess {i+1}: ");
                    if (!Guess(Console.ReadLine().Split(' ')))
                        i--;
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
                        Console.WriteLine("   " + string.Join(" ", wordles[i].Answers));
                } else if (wordles[i].NumAnswers == 1)
                    Console.WriteLine(wordles[i].Answers[0]);
                else Console.WriteLine("No answer");
            }
        }

        private bool Guess(string[] guess)
        {
            int i, offset = 1;
            for (i = 0; i < wordles.Length; i++)
                if (wordles[i].NumAnswers > 1)
                    offset++;
            if (guess.Length != offset) return false;
            for (i = 0; i < guess.Length; i++)
                if (guess[i].Length != 5) return false;
            offset = 1;
            for (i = 0; i < wordles.Length; i++)
                if (wordles[i].NumAnswers > 1)
                    wordles[i].Guess(new string[] { guess[0], guess[i + offset] });
                else offset--;
            return true;
        }
        private string Suggest(bool print = true)
        {
            int i;
            for (i = 0; i < wordles.Length; i++) {
                wordles[i].Frequency(false, false);
                wordles[i].ExcludeLetters();
            }
            IOrderedEnumerable<string> sorted = guesses.OrderByDescending(ScoreWord);
            if (print) {
                Console.Write("Suggestions:");
                int j = 0;
                foreach (string word in sorted) {
                    if (j++ >= 10) break;
                    Console.Write(" " + word);
                }
                Console.WriteLine();
            }
            return sorted.ElementAt(0);
        }
        public int ScoreWord(string word)
        {
            int i, score = 0;
            for (i = 0; i < wordles.Length; i++)
                if (wordles[i].NumAnswers > 1)
                    score += (int)Math.Round(2.0 * wordles[i].NumAnswers / 2315 * wordles[i].ScoreWord(word));
            return score;
        }
    }
}
