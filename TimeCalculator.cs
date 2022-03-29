using System;

namespace MiscConsole
{
    class TimeCalculator
    {
        public void Main(string[] args)
        {
            string line;
            int i;
            int[] time = new int[6], dt;
            do {
                for (i = 0; i < time.Length; i++)
                    time[i] = 0;
                line = Console.ReadLine();
                while ((i = line.IndexOf('=')) == -1) {
                    dt = Parse(line);
                    Add(time, dt);
                    line = Console.ReadLine();
                }
                dt = Parse(line.Substring(0, i));
                Add(time, dt);
                Print(time);
            } while (MiscConsole.Continue("\nAnother calculation?"));
        }

        protected int[] Parse(string line)
        {
            string timeString, times, time;
            int i, j;
            bool add;
            char unit;
            int[] curr = new int[6], total = new int[6];
            line = line.Trim().ToLower();
            // split up entries
            while (line.Length > 0) {
                add = line[0] != '-';
                j = line[0] == '+' || line[0] == '-' ? 1 : 0;
                i = line.Substring(j).IndexOfAny(new char[] { '-', '+' });
                if (i == -1) {
                    timeString = line.Substring(j).Trim();
                    line = "";
                } else {
                    timeString = line.Substring(j, i).Trim();
                    line = line.Substring(i).Trim();
                }
                curr[0] = curr[1] = curr[2] = curr[3] = curr[4] = curr[5] = 0;
                // parse single entry
                do {
                    for (i = 0; i < timeString.Length; i++)
                        if (char.IsLetter(timeString[i]))
                            break;
                        else if (char.IsWhiteSpace(timeString[i]))
                            timeString.Remove(i--);
                    times = timeString.Substring(0, i);
                    unit = timeString[i];
                    timeString = i < timeString.Length-1 ? timeString.Substring(i+1) : "";
                    //parse time amounts
                    do {
                        i = times.IndexOf(':');
                        if (i == -1) {
                            time = times;
                            times = "";
                        } else {
                            time = times.Substring(0, i);
                            times = i < times.Length - 1 ? times.Substring(i + 1) : "";
                        }
                        if (int.TryParse(time, out i))
                            switch (unit) {
                                case 'y': curr[0] += i; unit = 'd'; break;
                                case 'd': curr[1] += i; unit = 'h'; break;
                                case 'h': curr[2] += i; unit = 'm'; break;
                                case 'm': curr[3] += i; unit = 's'; break;
                                case 's': curr[4] += i; unit = 'u'; break;
                                default: /*'u'*/ curr[5] = i; break;
                            }
                    } while (times.Length > 0);
                } while (timeString.Length > 0);
                // add current to total
                if (!add)
                    for (i = 0; i < curr.Length; i++)
                        curr[i] *= -1;
                Add(total, curr);
            }
            return total;
        }

        protected void Add(int[] total, int[] addition)
        {
            int[] max = new int[]{ 365, 24, 60, 60, 1000 };
            int i, sign = 0, start = 0;
            for (i = total.Length - 1; i > 0; i--) {
                total[i] += addition[i];
                if (Math.Abs(total[i]) >= max[i-1]) {
                    total[i-1] += total[i] / max[i-1];
                    total[i] %= max[i - 1];
                }
            }
            total[0] += addition[0];
            for (i = 0; i < total.Length; i++)
                if (total[i] != 0) {
                    start = i;
                    sign = Math.Sign(total[i]);
                    break;
                }
            if (sign != 0)
                for (i = total.Length - 1; i > start; i--)
                    while (total[i] != 0 && sign != Math.Sign(total[i])) {
                        total[i - 1] += Math.Sign(total[i]);
                        total[i] -= Math.Sign(total[i]) * max[i - 1];
                    }
        }

        protected void Print(int[] time)
        {
            int start, end;
            char unit;
            for (start = 0; start < time.Length; start++)
                if (time[start] != 0) break;
            if (start == time.Length) {
                Console.WriteLine("0u");
                return;
            }
            for (end = time.Length - 1; end >= start; end--)
                if (time[end] != 0) break;
            switch (start) {
                case 0: unit = 'y'; break;
                case 1: unit = 'd'; break;
                case 2: unit = 'h'; break;
                case 3: unit = 'm'; break;
                case 4: unit = 's'; break;
                default: unit = 'u'; break;
            }
            Console.Write(time[start]);
            for (start++; start <= end; start++)
                Console.Write(":" + time[start]);
            Console.WriteLine(unit);
        }
    }
}
