using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiscConsole
{
    class ParseWordle
    {
        static List<string> answers, guesses;
        static readonly string ansFile = "..\\..\\wordle.txt",
            guessFile = "..\\..\\guesses.txt",
            alphabet = "abcdefghijklmnopqrstuvwxyz";
        static string[] letters;
        static string required, exclude;
        static bool writeGuesses;

        static void Main(string[] args)
        {
            guesses = new List<string>();
            answers = new List<string>();
            writeGuesses = false;
            while (true) {
                //setup
                answers.Clear();
                guesses.Clear();
                answers.AddRange(File.ReadAllLines(ansFile));
                guesses.AddRange(answers);
                guesses.AddRange(File.ReadAllLines(guessFile));
                letters = new string[] { alphabet, alphabet, alphabet, alphabet, alphabet };
                required = exclude = "";
                //game play
                for (int i = 0; i < 5 && answers.Count > 1; i++) {
                    Console.WriteLine(answers.Count + " possible answers");
                    if (answers.Count < 50)
                        Console.WriteLine(string.Join(" ", answers));
                    Frequency();
                    Console.Write("Enter guess: ");
                    Guess(Console.ReadLine().Split(' '));
                    Console.WriteLine();
                }
                if (answers.Count == 0) Console.WriteLine("No answer.");
                else Console.WriteLine("Answer: " + answers[0]);
                Console.Write("Another game? (y/n): ");
                if (Console.ReadLine() != "y")
                    break;
                Console.WriteLine("\n");
            }
            if (writeGuesses)
                File.WriteAllLines(guessFile, guesses);
        }

        static void Guess(string[] guess)
        {
            if (guess.Length != 2 || guess[0].Length != 5 || guess[1].Length != 5)
                return;
            if (!guesses.Contains(guess[0])) {
                guesses.Add(guess[0]);
                writeGuesses = true;
            }
            int k;
            for (int i = 0; i < 5; i++)
                switch(guess[1][i]) {
                    case 'n':       //check if the same letter has been marked g/y elsewhere in guess
                        if ((k = guess[0].IndexOf(guess[0][i])) != i && (guess[1][k] != 'n') ||
                            (k = guess[0].LastIndexOf(guess[0][i])) != i && (guess[1][k] != 'n'))
                            letters[i] = letters[i].Replace("" + guess[0][i], "");
                        else {
                            if (!exclude.Contains(guess[0][i]))
                                exclude += guess[0][i];
                            for (int j = 0; j < 5; j++)
                                letters[j] = letters[j].Replace("" + guess[0][i], "");
                        }
                        break;
                    case 'y':
                        letters[i] = letters[i].Replace("" + guess[0][i], "");
                        if (!required.Contains(guess[0][i]))
                            required += guess[0][i];
                        break;
                    case 'g':
                        letters[i] = "" + guess[0][i];
                        //if (!required.Contains(guess[0][i]))
                        //    required += guess[0][i];
                        break;
                }
            Filter(answers, required, letters);
        }
        static void Filter(List<string> words, string required, string[] letters)
        {
            bool removed;
            for (int i = 0; i < words.Count; i++) {
                removed = false;
                for (int j = 0; j < 5; j++)
                    if (!letters[j].Contains(words[i][j])) {
                        removed = true;
                        words.RemoveAt(i--);
                        break;
                    }
                if (!removed)
                    for (int j = 0; j < required.Length; j++)
                        if (!words[i].Contains(required[j])) {
                            words.RemoveAt(i--);
                            break;
                        }
            }
        }
        static void Frequency()
        {
            Dictionary<char, int> letFreq = new Dictionary<char, int>();
            for (char c = 'a'; c <= 'z'; c++)
                letFreq.Add(c, 0);
            for (int i = 0; i < answers.Count; i++)
                for (int j = 0; j < 5; j++)
                    letFreq[answers[i][j]]++;
            int k = 0;
            foreach (KeyValuePair<char,int> keyVal in letFreq.OrderByDescending(key => key.Value)) {
                if (k++ >= 10 || (double)keyVal.Value / answers.Count < .05) break;
                Console.Write(keyVal.Key + ": " + ((double)keyVal.Value / answers.Count * 20).ToString("g3") + "%  ");
            }
            Console.WriteLine();
            Suggest(letFreq);
        }
        static void Suggest(Dictionary<char, int> letFreq)
        {
            string greens = "";
            string[] tempLet = new string[5];
            Array.Copy(letters, tempLet, 5);
            for (int i = 0; i < 5; i++) 
                if (tempLet[i].Length == 1) {
                    greens += tempLet[i];
                    tempLet[i] = alphabet;
                }
            for (int i = 0; i < 5; i++) 
                foreach (char c in greens + exclude)
                    tempLet[i] = tempLet[i].Replace("" + c, "");
            IOrderedEnumerable<string> sorted = guesses.OrderByDescending(word => {
                int count = 0;
                for (int i = 0; i < word.Length; i++)
                    if (word.IndexOf(word[i]) == i && tempLet[i].Contains(word[i]))
                        count += letFreq[word[i]];
                return count;
            });
            Console.Write("Suggestions:");
            int j = 0;
            foreach (string word in sorted) {
                if (j++ >= 10) break;
                Console.Write(" " + word);
            }
            Console.WriteLine();
        }
    }
}
