using System.Reflection;

namespace Boink.Types
{
    public sealed class StandardFunctionType : ObjectType
    {
        public StandardFunctionType(string name, object val) : base(name, val)
        {

        }

        public ObjectType InvokeFunction(object[] args)
        {
            return (ObjectType)((MethodInfo)Val).Invoke(null, args);
        }

        public override ObjectType DeepCopy()
        {
            StandardFunctionType newObj = new StandardFunctionType(this.Name, this.Val);
            return newObj;
        }
    }
}
