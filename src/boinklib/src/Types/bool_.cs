namespace Boink.Types
{

    public sealed class bool_ : obj_
    {
        public bool_(string name, object val) : base(name, val) { }

        public override object and(obj_ other)
        {
            return new bool_(null, (bool)Val && (bool)other.Val);
        }

        public override object or(obj_ other)
        {
            return new bool_(null, (bool)Val || (bool)other.Val);
        }
    }

}
