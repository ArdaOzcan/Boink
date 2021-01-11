namespace Boink.Types
{

    public sealed class BoolType : ObjectType
    {
        public BoolType(string name, object val) : base(name, val) { }

        public override object and(ObjectType other)
        {
            return new BoolType(null, (bool)Val && (bool)other.Val);
        }

        public override object or(ObjectType other)
        {
            return new BoolType(null, (bool)Val || (bool)other.Val);
        }

        public override ObjectType DeepCopy()
        {
            BoolType newObj = new BoolType(this.Name, this.Val);
            return newObj;
        }
    }

}
