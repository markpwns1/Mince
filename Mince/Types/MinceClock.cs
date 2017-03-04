
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    [StaticClass("clock")]
    public class MinceClock : MinceObject
    {
        public MinceClock()
        {
            CreateMembers();
        }

        public override string ToString()
        {
            return new MinceDate(DateTime.Now).ToString() + " " + new MinceTime(DateTime.Now).ToString(); 
        }

        [Exposed]
        public MinceDate today
        {
            get { return new MinceDate(DateTime.Now); }
        }

        [Exposed]
        public MinceTime currentTime
        {
            get { return new MinceTime(DateTime.Now); }
        }

        [Exposed]
        public MinceNull wait(MinceNumber seconds)
        {
            System.Threading.Thread.Sleep((int)(Convert.ToSingle(seconds.value) * 1000f));
            return new MinceNull();
        }
    }
}
