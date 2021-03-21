using System.Reflection;

namespace Boink.Types
{
    public sealed class StandardFunctionType : ObjectType
    {
        public object Target { get; set; }
        public bool FirstArgIsInstance { get; }

        public StandardFunctionType(string name, object val, bool firstArgIsInstance = false) : base(name, val)
        {
            FirstArgIsInstance = firstArgIsInstance;
        }

        public ObjectType InvokeFunction(object[] args)
        {
            if (FirstArgIsInstance)
                args[0] = Target;

            return (ObjectType)((MethodInfo)Val).Invoke(Target, args);
        }

        public override ObjectType DeepCopy()
        {
            StandardFunctionType newObj = new StandardFunctionType(this.Name, this.Val);
            return newObj;
        }
    }
}
