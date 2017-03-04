using System.Diagnostics;

namespace Mince.Types
{
    [Instantiatable("Stopwatch")]
    public class MinceStopWatch : MinceObject
    {
        [Exposed]
        public MinceNumber elapsedTime
        {
            get { return new MinceNumber(GetStopWatch().ElapsedMilliseconds); }
        }

        public MinceStopWatch()
        {
            this.value = new Stopwatch();
            CreateMembers();
        }

        [Exposed]
        public MinceNull start()
        {
            GetStopWatch().Start();
            return new MinceNull();
        }

        [Exposed]
        public MinceNumber pause()
        {
            GetStopWatch().Stop();
            return elapsedTime;
        }

        [Exposed]
        public MinceNumber stop()
        {
            GetStopWatch().Stop();
            return elapsedTime;
        }

        public Stopwatch GetStopWatch()
        {
            return this.value as Stopwatch;
        }
    }
}
