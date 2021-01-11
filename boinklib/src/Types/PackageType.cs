namespace Boink.Types
{
    public sealed class PackageType : ObjectType
    {
        public PackageType(string name, object val) : base(name, val)
        {

        }

        public override ObjectType DeepCopy()
        {
            PackageType newObj = new PackageType(this.Name, this.Val);
            return newObj;
        }
    }
}
