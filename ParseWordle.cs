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
        public int MaxGuess = 6;
        public Dictionary<char, int> LetFreq;
        public int NumAnswers { get { return Answers != null ? Answers.Count : 0; } }
        public List<string> Answers;

        List<string> guesses;
        const string ansFile = "..\\..\\wordle.txt",
            guessFile = "..\\..\\guesses.txt",
            alphabet = "abcdefghijklmnopqrstuvwxyz";
        string[] letters, exLet;
        byte[] required;
        string exclude;

        public ParseWordle(List<string> guess = null)
        {
            guesses = guess;
            Answers = new List<string>();
            LetFreq = new Dictionary<char, int>(26);
            for (char c = 'a'; c <= 'z'; c++)
                LetFreq.Add(c, 0);
        }

        public void Main(string[] args)
        {
            if (args.Length >= 2 && args[1].ToLower() == "test") {
                List<string> words = new List<string>(File.ReadAllLines(ansFile));
                StreamWriter sw = new StreamWriter(File.OpenWrite("..\\..\\testing.txt"));
                string[] guess = new string[2];
                int[] numTries = new int[7];
                int i;
                foreach (string word in words) {
                    Console.WriteLine(word);
                    sw.WriteLine(word + "\n----------------");
                    Setup();
                    guess[1] = Compare(word, guess[0] = Frequency(false));
                    for (i = 0; i < 6 && guess[0] != word; i++) {
                        sw.WriteLine($"{guess[0]} {guess[1]} {Answers.Count}");
                        Guess(guess);
                        guess[1] = Compare(word, guess[0] = Frequency(false));
                    }
                    sw.WriteLine($"{guess[0]} {guess[1]} {Answers.Count}");
                    sw.WriteLine();
                    numTries[i]++;
                }
                sw.WriteLine("Results: " + string.Join(", ", numTries));
                sw.Close();
                Console.WriteLine("Testing complete!");
            } else {
                while (true) {
                    Setup();
                    for (int i = 0; i < MaxGuess - 1 && Answers.Count > 1; i++) {
                        Console.WriteLine(Answers.Count + " possible answers");
                        if (Answers.Count < 50)
                            Console.WriteLine(string.Join(" ", Answers));
                        Frequency(true);
                        Console.Write("Enter guess: ");
                        Guess(Console.ReadLine().Split(' '));
                        Console.WriteLine();
                    }
                    if (Answers.Count == 0) Console.WriteLine("No answer.");
                    else Console.WriteLine("Answer: " + Answers[0]);
                    if (!MiscConsole.Continue("Another game?"))
                        break;
                    Console.WriteLine("\n");
                }
            }
        }
        public void Setup()
        {
            Answers.Clear();
            Answers.AddRange(File.ReadAllLines(ansFile));
            if (guesses == null) {
                guesses = new List<string>();
                guesses.AddRange(File.ReadAllLines(guessFile));
                for (int i = 0; i < Answers.Count; i++)
                    if (!guesses.Contains(Answers[i]))
                        guesses.Add(Answers[i]);
            }
            letters = new string[] { alphabet, alphabet, alphabet, alphabet, alphabet };
            required = new byte[26];
            exclude = "";
        }
        private string Compare(string word, string guess)
        {
            string comp = "nnnnn";
            byte[] wlets = new byte[26];
            for (int i = 0; i < 5; i++) {
                if (guess[i] == word[i])
                    comp = comp.Substring(0,i) + 'g' + comp.Substring(i+1);
                else
                    wlets[word[i] - 'a']++;
            }
            for (int i = 0; i < 5; i++)
                if (comp[i] == 'n' && wlets[guess[i]-'a'] > 0) {
                    comp = comp.Substring(0,i) + 'y' + comp.Substring(i+1);
                    wlets[guess[i] - 'a']--;
                }
            return comp;
        }

        public void Guess(string[] guess)
        {
            if (guess.Length != 2 || guess[0].Length != 5 || guess[1].Length != 5)
                return;
            int k, cnt;
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
                        cnt = 0;
                        for (int j = 0; j < 5; j++)
                            if (guess[0][j] == guess[0][i] && guess[1][j] != 'n')
                                cnt++;
                        if (cnt > required[guess[0][i] - 'a'])
                            required[guess[0][i] - 'a'] = (byte)cnt;
                        break;
                    case 'g':
                        letters[i] = "" + guess[0][i];
                        break;
                }
            Filter(Answers, required, letters);
        }
        private void Filter(List<string> words, byte[] required, string[] letters)
        {
            byte cnt;
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
                    for (char c = 'a'; c <= 'z'; c++) {
                        if (required[c - 'a'] == 0)
                            continue;
                        cnt = 0;
                        for (byte j = 0; j < 5; j++)
                            if (words[i][j] == c)
                                cnt++;
                        if (cnt < required[c - 'a']) {
                            words.RemoveAt(i--);
                            break;
                        }
                    }
            }
        }
        public string Frequency(bool print, bool suggest = true)
        {
            for (char c = 'a'; c <= 'z'; c++)
                LetFreq[c] = 0;
            for (int i = 0; i < Answers.Count; i++)
                for (int j = 0; j < 5; j++)
                    LetFreq[Answers[i][j]]++;
            if (print) {
                int k = 0;
                foreach (KeyValuePair<char, int> keyVal in LetFreq.OrderByDescending(key => key.Value)) {
                    if (k++ >= 10 || (double)keyVal.Value / Answers.Count < .05) break;
                    Console.Write(keyVal.Key + ": " + ((double)keyVal.Value / Answers.Count * 20).ToString("g3") + "%  ");
                }
                Console.WriteLine();
            }
            return suggest ? Suggest(LetFreq, print) : null;
        }
        private string Suggest(Dictionary<char, int> letFreq, bool print)
        {
            ExcludeLetters();
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

        public string[] ExcludeLetters()
        {
            string greens = "";
            exLet = new string[5];
            Array.Copy(letters, exLet, 5);
            for (int i = 0; i < 5; i++) 
                if (exLet[i].Length == 1) {
                    greens += exLet[i];
                    exLet[i] = alphabet;
                }
            for (int i = 0; i < 5; i++) 
                foreach (char c in greens + exclude)
                    exLet[i] = exLet[i].Replace("" + c, "");
            return exLet;
        }
        public int ScoreWord(string word)
        {
            int count = 0;
            bool[] letChk = new bool[26];
            for (int i = 0; i < word.Length; i++)
                if (!letChk[word[i] - 'a'] && exLet[i].Contains(word[i])) {
                    count += LetFreq[word[i]];
                    letChk[word[i] - 'a'] = true;
                }
            if (Answers.Contains(word))
                count += 5;
            return count;
        }
    }
}
