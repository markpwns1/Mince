
namespace Mince.Types
{
    public class MinceBool : MinceClonable
    {
        public MinceBool(bool value)
        {
            this.value = value;
            CreateMembers();
        }

        [Exposed]
        public MinceNull invert()
        {
            this.value = !(bool)this.value;
            return new MinceNull();
        }

        [Exposed]
        public MinceBool inverted()
        {
            return new MinceBool(!(bool)this.value);
        }

        public override string ToString()
        {
            return base.ToString().ToLower();
        }

        [Exposed]
        public override MinceObject clone()
        {
            return new MinceBool(this.ToBool());
        }

        public bool ToBool()
        {
            return (bool)this.value;
        }
    }
}
