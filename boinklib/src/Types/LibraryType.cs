namespace Boink.Types
{
    public sealed class LibraryType : ObjectType
    {
        public LibraryType(string name, object val) : base(name, val)
        {

        }

        public override ObjectType DeepCopy()
        {
            LibraryType newObj = new LibraryType(this.Name, this.Val);
            return newObj;
        }
    }
}
