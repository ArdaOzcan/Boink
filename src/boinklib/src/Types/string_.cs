namespace Boink.Types
{
    public sealed class string_ : obj_
    {
        public string_(string name, object val) : base(name, val)
        {

        }

        public override object add(obj_ other)
        {
            return new string_(null, (string)Val + (string)other.Val);
        }
    }

}
