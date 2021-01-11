namespace Boink.Types
{
    public sealed class StringType : ObjectType
    {
        public StringType(string name, object val) : base(name, val)
        {

        }

        public override object add(ObjectType other)
        {
            return new StringType(null, (string)Val + (string)other.Val);
        }

        public override ObjectType DeepCopy()
        {
            StringType newObj = new StringType(this.Name, this.Val);
            return newObj;
        }
    }

}
