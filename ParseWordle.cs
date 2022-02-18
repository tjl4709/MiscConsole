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
        static List<string> words, backup;
        static readonly string inFile = "..\\..\\wordle.txt",
            outFile = "..\\..\\guesses.txt",
            alphabet = "abcdefghijklmnopqrstuvwxyz";
        static string[] letters;
        static string required;
        static bool writeBackup;

        static void Main(string[] args)
        {
            backup = new List<string>();
            words = new List<string>();
            backup.AddRange(File.ReadAllLines(inFile));
            writeBackup = false;
            while (true) {
                words.Clear();
                letters = new string[] { alphabet, alphabet, alphabet, alphabet, alphabet };
                words.AddRange(backup);
                for (int i = 0; i < 5 && words.Count > 1; i++) {
                    Console.WriteLine(words.Count + " possible guesses");
                    if (words.Count < 50)
                        Console.WriteLine(string.Join(" ", words));
                    Frequency();
                    Console.Write("Enter guess: ");
                    Guess(Console.ReadLine().Split(' '));
                    Filter();
                    Console.WriteLine();
                }
                if (words.Count == 0) Console.WriteLine("No answer.");
                else Console.WriteLine("Answer: " + words[0]);
                Console.Write("Another game? (y/n): ");
                if (Console.ReadLine() != "y")
                    break;
                Console.WriteLine("\n");
            }
            if (writeBackup)
                File.WriteAllLines(inFile, backup);
        }

        static void Guess(string[] guess)
        {
            if (guess.Length != 2 || guess[0].Length != 5 || guess[1].Length != 5)
                return;
            if (!backup.Contains(guess[0])) {
                backup.Add(guess[0]);
                writeBackup = true;
            }
            required = "";
            int k;
            for (int i = 0; i < 5; i++)
                switch(guess[1][i]) {
                    case 'n':       //check if the same letter has been marked g/y elsewhere in guess
                        if ((k=guess[0].IndexOf(guess[0][i])) != i && (guess[1][k] != 'n') ||
                            (k=guess[0].LastIndexOf(guess[0][i])) != i && (guess[1][k] != 'n'))
                            letters[i] = letters[i].Replace("" + guess[0][i], "");
                        else
                            for (int j = 0; j < 5; j++)
                                letters[j] = letters[j].Replace("" + guess[0][i], "");
                        break;
                    case 'y':
                        letters[i] = letters[i].Replace("" + guess[0][i], "");
                        required += guess[0][i];
                        break;
                    case 'g':
                        letters[i] = "" + guess[0][i];
                        break;
                }
        }
        static void Filter()
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
            File.WriteAllText(outFile, string.Join(" ", words));
        }
        static void Frequency()
        {
            Dictionary<char, int> letFreq = new Dictionary<char, int>();
            for (char c = 'a'; c <= 'z'; c++)
                letFreq.Add(c, 0);
            for (int i = 0; i < words.Count; i++)
                for (int j = 0; j < 5; j++)
                    letFreq[words[i][j]]++;
            int k = 0;
            foreach (KeyValuePair<char,int> keyVal in letFreq.OrderByDescending(key => key.Value)) {
                if (k >= 10 || (double)keyVal.Value / words.Count < .05) break;
                Console.Write(keyVal.Key + ": " + ((double)keyVal.Value / words.Count * 20).ToString("g3") + "%  ");
                k++;
            }
            Console.WriteLine();
        }
    }
}
