using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private StringWriter writer;
        private Timer father;

        public List<Tuple<string, long, byte>> timersDuration = new List<Tuple<string, long, byte>>(); // the first value is this timer name and duration 
        public Timer(StringWriter writer, string timerName, Timer father)
        {
            this.writer = writer;
            this.father = father;

            long time= (long)(DateTime.Now.Ticks / 10000.0);
            timersDuration.Add(new Tuple<string, long, byte>(timerName, time, 0) );
        }

        public static Timer Start(StringWriter writer, string name) => new Timer(writer, name, null);
        public static Timer Start(StringWriter writer) => Start(writer, "*");

        public Timer StartChildTimer(string name)
        {
            return new Timer(new StringWriter(), name, this);
        }


        // Use this method in your solution to fit report formatting requirements from the tests
        private static string FormatReportLine(string timerName, int level, long value)
        {
            var intro = new string(' ', level * 4) + timerName;
            return $"{intro,-20}: {value}\n";
        }

        void IDisposable.Dispose()
        {
            long duration = (long)(DateTime.Now.Ticks / 10000.0 - timersDuration[0].Item2);
            timersDuration[0] = new Tuple<string, long, byte>(timersDuration[0].Item1, duration, timersDuration[0].Item3);

            if(timersDuration.Count > 1)
            {
                long timeSum = 0;
                foreach(var tuple in timersDuration)
                {
                    if(! tuple.Equals(timersDuration[0]))
                    {
                        timeSum += tuple.Item2;
                    }
                }

                long restDuration = timersDuration.First().Item2- timeSum;
                timersDuration.Add(new Tuple<string, long, byte>("Rest", restDuration, (byte)(timersDuration[0].Item3 + 1)) );
            }

            if (father != null)
            {
                foreach (var tuple in timersDuration)
                    father.timersDuration.Add(new Tuple<string, long, byte>(tuple.Item1, tuple.Item2, (byte)(tuple.Item3 + 1)) );
            }

            else
            {
                foreach (var tuple in timersDuration)
                    writer.Write(FormatReportLine(tuple.Item1, tuple.Item3, tuple.Item2));
            }
        }
    }
}
