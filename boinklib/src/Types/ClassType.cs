namespace Boink.Types
{
    public sealed class ClassType : ObjectType
    {
        public ClassType(string name, object val) : base(name, val)
        {

        }

        public override ObjectType DeepCopy()
        {
            ClassType newObj = new ClassType(this.Name, this.Val);
            return newObj;
        }
    }
}
