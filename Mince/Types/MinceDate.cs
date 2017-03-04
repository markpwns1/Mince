
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    [Instantiatable("Date")]
    public class MinceDate : MinceObject
    {
        [Exposed]
        public MinceNumber year
        {
            get { return new MinceNumber(GetValue().Year); }
        }

        [Exposed]
        public MinceNumber month
        {
            get { return new MinceNumber(GetValue().Month); }
        }

        [Exposed]
        public MinceNumber day
        {
            get { return new MinceNumber(GetValue().Day); }
        }

        public MinceDate(MinceNumber day, MinceNumber month, MinceNumber year)
        {
            this.value = new DateTime(year.ToInt(), month.ToInt(), day.ToInt());
            CreateMembers();
        }

        public MinceDate(DateTime date)
        {
            this.value = date;
            CreateMembers();
        }

        public MinceDate()
        {
            this.value = new DateTime(1, 1, 1);
            CreateMembers();
        }

        public override MinceObject Plus(MinceObject other)
        {
            DateTime d = new DateTime(0, 0, 0);

            d.AddYears(GetValue().Year);
            d.AddMonths(GetValue().Month);
            d.AddDays(GetValue().Day);

            DateTime otherDate = (DateTime)other.value;

            d.AddYears(otherDate.Year);
            d.AddMonths(otherDate.Month);
            d.AddDays(otherDate.Day);

            return new MinceDate(d);
        }

        public override MinceObject Minus(MinceObject other)
        {
            DateTime d = new DateTime(0, 0, 0);

            d.AddYears(-GetValue().Year);
            d.AddMonths(-GetValue().Month);
            d.AddDays(-GetValue().Day);

            DateTime otherDate = (DateTime)other.value;

            d.AddYears(-otherDate.Year);
            d.AddMonths(-otherDate.Month);
            d.AddDays(-otherDate.Day);

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
            return day + "/" + month + "/" + year;
        }

        public DateTime GetValue()
        {
            return (DateTime)this.value;
        }
    }
}
