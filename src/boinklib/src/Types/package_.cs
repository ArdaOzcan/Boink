namespace Boink.Types
{
    public sealed class package_ : obj_
    {
        public package_(string name, object val) : base(name, val)
        {

        }

        public override obj_ DeepCopy()
        {
            package_ newObj = new package_(this.Name, this.Val);
            return newObj;
        }
    }
}
