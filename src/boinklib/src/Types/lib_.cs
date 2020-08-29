namespace Boink.Types
{
    public sealed class lib_ : obj_
    {
        public lib_(string name, object val) : base(name, val)
        {

        }

        public override obj_ DeepCopy()
        {
            lib_ newObj = new lib_(this.Name, this.Val);
            return newObj;
        }
    }
}
