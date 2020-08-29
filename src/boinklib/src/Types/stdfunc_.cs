using System.Reflection;

namespace Boink.Types
{
    public sealed class stdfunc_ : obj_
    {
        public stdfunc_(string name, object val) : base(name, val)
        {

        }

        public obj_ InvokeFunction(object[] args)
        {
            return (obj_)((MethodInfo)Val).Invoke(null, args);
        }

        public override obj_ DeepCopy()
        {
            stdfunc_ newObj = new stdfunc_(this.Name, this.Val);
            return newObj;
        }
    }
}
