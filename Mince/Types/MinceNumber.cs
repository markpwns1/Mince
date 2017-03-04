
using System;
using System.Reflection;

namespace Mince.Types
{
    public class MinceNumber : MinceClonable
    {
        public MinceNumber(float value)
        {
            this.value = value;
            CreateMembers();
        }

        public MinceNumber()
        {
            this.value = 0;
            CreateMembers();
        }

        #region operators
        public override MinceObject Plus(MinceObject other)
        {
            return new MinceNumber((float)this.value + (float)other.value);
        }

        public override MinceObject Minus(MinceObject other)
        {
            return new MinceNumber((float)this.value - (float)other.value);
        }

        public override MinceObject Multiply(MinceObject other)
        {
            return new MinceNumber((float)this.value * (float)other.value);
        }

        public override MinceObject Divide(MinceObject other)
        {
            return new MinceNumber((float)this.value / (float)other.value);
        }

        public override MinceObject Exponent(MinceObject other)
        {
            return new MinceNumber((float)Math.Pow((float)this.value, (float)other.value));
        }

        public override MinceBool GreaterThan(MinceObject other)
        {
            return new MinceBool((float)this.value > (float)other.value);
        }

        public override MinceBool LessThan(MinceObject other)
        {
            return new MinceBool((float)this.value < (float)other.value);
        }

        public override MinceBool GreaterOrEqual(MinceObject other)
        {
            return new MinceBool((float)this.value >= (float)other.value);
        }

        public override MinceBool LessOrEqual(MinceObject other)
        {
            return new MinceBool((float)this.value <= (float)other.value);
        }


        #endregion

        #region members
        [Exposed]
        public MinceNumber abs()
        {
            return new MinceNumber((float)Math.Abs((float)this.value));
        }

        [Exposed]
        public MinceNumber acos()
        {
            return new MinceNumber((float)Math.Acos((float)this.value));
        }

        [Exposed]
        public MinceNumber asin()
        {
            return new MinceNumber((float)Math.Asin((float)this.value));
        }

        [Exposed]
        public MinceNumber atan()
        {
            return new MinceNumber((float)Math.Atan((float)this.value));
        }

        [Exposed]
        public MinceNumber atan2(MinceNumber other)
        {
            return new MinceNumber((float)Math.Atan2((float)this.value, (float)other.value));
        }

        [Exposed]
        public MinceNumber ciel()
        {
            return new MinceNumber((float)Math.Ceiling((float)this.value));
        }

        [Exposed]
        public MinceNumber clamp(MinceNumber min, MinceNumber max)
        {
            return this.min(max).max(min);
        }

        [Exposed]
        public MinceNumber cos()
        {
            return new MinceNumber((float)Math.Cos((float)this.value));
        }

        [Exposed]
        public MinceNumber cosh()
        {
            return new MinceNumber((float)Math.Cosh((float)this.value));
        }

        [Exposed]
        public MinceNumber floor()
        {
            return new MinceNumber((float)Math.Floor((float)this.value));
        }

        [Exposed]
        public MinceNumber fPart(MinceNumber input)
        {
            return (MinceNumber)input.Minus(input.truncate());
        }

        [Exposed]
        public MinceNull inc()
        {
            this.value = (float)this.value + 1f;
            return new MinceNull();
        }

        [Exposed]
        public MinceNumber lerp(MinceNumber other, MinceNumber delta)
        {
            return (MinceNumber)this.Plus(other.Multiply(delta));
        }

        [Exposed]
        public MinceNumber log()
        {
            return new MinceNumber((float)Math.Log((float)this.value));
        }

        [Exposed]
        public MinceNumber log10()
        {
            return new MinceNumber((float)Math.Log10((float)this.value));
        }

        [Exposed]
        public MinceNumber max(MinceNumber other)
        {
            return new MinceNumber((float)Math.Max((float)this.value, (float)other.value));
        }

        [Exposed]
        public MinceNumber min(MinceNumber other)
        {
            return new MinceNumber((float)Math.Min((float)this.value, (float)other.value));
        }

        [Exposed]
        public MinceNumber mod(MinceNumber other)
        {
            return new MinceNumber(this.ToFloat() % other.ToFloat());
        }

        [Exposed]
        public MinceNumber round()
        {
            return new MinceNumber((float)Math.Round((float)this.value));
        }

        [Exposed]
        public MinceNumber sin()
        {
            return new MinceNumber((float)Math.Sin((float)this.value));
        }

        [Exposed]
        public MinceNumber sinh()
        {
            return new MinceNumber((float)Math.Sinh((float)this.value));
        }

        [Exposed]
        public MinceNumber sqrt()
        {
            return new MinceNumber((float)Math.Sqrt((float)this.value));
        }

        [Exposed]
        public MinceNumber tan()
        {
            return new MinceNumber((float)Math.Tan((float)this.value));
        }

        [Exposed]
        public MinceNumber tanh()
        {
            return new MinceNumber((float)Math.Tanh((float)this.value));
        }

        [Exposed]
        public MinceNumber truncate()
        {
            return new MinceNumber((float)Math.Truncate((float)this.value));
        }

        [Exposed]
        public MinceByte toByte()
        {
            return new MinceByte((byte)(ToInt() % byte.MaxValue));
        }
        #endregion

        public override MinceObject clone()
        {
            return new MinceNumber(this.ToFloat());
        }

        public float ToFloat()
        {
            return Convert.ToSingle(this.value);
        }

        public int ToInt()
        {
            return Convert.ToInt32(this.value);
        }

    }
}
