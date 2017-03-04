
using System;

namespace Mince.Types
{
    [StaticClass("random")]
    public class MinceRandom : MinceObject
    {
        private Random r = new Random();

        public MinceRandom()
        {
            CreateMembers();
        }

        [Exposed]
        public MinceNumber next()
        {
            return new MinceNumber((float)r.NextDouble());
        }

        [Exposed]
        public MinceNumber between(MinceNumber min, MinceNumber max)
        {
            float mi = (float)min.value;
            float ma = (float)max.value;
            return new MinceNumber((float)(mi + r.NextDouble() * (ma - mi)));
        }

        [Exposed]
        public MinceNumber intBetween(MinceNumber min, MinceNumber max)
        {
            return between(min, max).round();
        }
    }
}
