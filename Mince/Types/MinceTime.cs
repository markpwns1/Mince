
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    [Instantiatable("Time")]
    public class MinceTime : MinceObject
    {
        [Exposed]
        public MinceNumber hour
        {
            get { return new MinceNumber(GetValue().Hour); }
        }

        [Exposed]
        public MinceNumber minute
        {
            get { return new MinceNumber(GetValue().Minute); }
        }

        [Exposed]
        public MinceNumber second
        {
            get { return new MinceNumber(GetValue().Second); }
        }

        public MinceTime(MinceNumber second, MinceNumber minute, MinceNumber hour)
        {
            this.value = new DateTime(0, 0, 0, hour.ToInt(), minute.ToInt(), second.ToInt());
            CreateMembers();
        }

        public MinceTime(DateTime date)
        {
            this.value = date;
            CreateMembers();
        }

        public MinceTime()
        {
            this.value = new DateTime(1, 1, 1, 1, 1, 1);
            CreateMembers();
        }

        public override MinceObject Plus(MinceObject other)
        {
            DateTime d = new DateTime(0, 0, 0);

            d.AddHours(GetValue().Hour);
            d.AddMinutes(GetValue().Minute);
            d.AddSeconds(GetValue().Second);

            DateTime otherDate = (DateTime)other.value;

            d.AddHours(otherDate.Hour);
            d.AddMinutes(otherDate.Minute);
            d.AddSeconds(otherDate.Second);

            return new MinceDate(d);
        }

        public override MinceObject Minus(MinceObject other)
        {
            DateTime d = new DateTime(0, 0, 0);

            d.AddHours(-GetValue().Hour);
            d.AddMinutes(-GetValue().Minute);
            d.AddSeconds(-GetValue().Second);

            DateTime otherDate = (DateTime)other.value;

            d.AddHours(-otherDate.Hour);
            d.AddMinutes(-otherDate.Minute);
            d.AddSeconds(-otherDate.Second);

            return new MinceDate(d);
        }

        public override MinceBool GreaterThan(MinceObject other)
        {
            return new MinceBool(GetValue() > (DateTime)other.value);
        }

        public override MinceBool LessThan(MinceObject other)
        {
            return new MinceBool(GetValue() < (DateTime)other.value);
        }

        public override string ToString()
        {
            return second + "/" + minute + "/" + hour;
        }

        public DateTime GetValue()
        {
            return (DateTime)this.value;
        }
    }
}
