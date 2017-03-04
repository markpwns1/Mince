using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceByte : MinceClonable
    {
        public MinceByte()
        {
            this.value = (byte)0;
        }

        public MinceByte(byte b)
        {
            this.value = b;
        }

        public override MinceObject clone()
        {
            return new MinceByte((byte)this.value);
        }

        public byte ToByte()
        {
            return (byte)this.value;
        }

        [Exposed]
        public MinceNumber toNumber()
        {
            return new MinceNumber((int)this.value);
        }

        #region operators
        public override MinceObject Plus(MinceObject other)
        {
            return new MinceNumber((byte)this.value + (byte)other.value);
        }

        public override MinceObject Minus(MinceObject other)
        {
            return new MinceNumber((byte)this.value - (byte)other.value);
        }

        public override MinceObject Multiply(MinceObject other)
        {
            return new MinceNumber((byte)this.value * (byte)other.value);
        }

        public override MinceObject Divide(MinceObject other)
        {
            return new MinceNumber((byte)this.value / (byte)other.value);
        }

        public override MinceObject Exponent(MinceObject other)
        {
            return new MinceNumber((byte)Math.Pow((byte)this.value, (byte)other.value));
        }

        public override MinceBool GreaterThan(MinceObject other)
        {
            return new MinceBool((byte)this.value > (byte)other.value);
        }

        public override MinceBool LessThan(MinceObject other)
        {
            return new MinceBool((byte)this.value < (byte)other.value);
        }

        public override MinceBool GreaterOrEqual(MinceObject other)
        {
            return new MinceBool((byte)this.value >= (byte)other.value);
        }

        public override MinceBool LessOrEqual(MinceObject other)
        {
            return new MinceBool((byte)this.value <= (byte)other.value);
        }
        #endregion
    }
}
