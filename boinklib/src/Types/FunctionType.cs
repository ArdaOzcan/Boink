using System.Collections.Generic;

using Boink.AST.Nodes;

namespace Boink.Types
{
    public sealed class FunctionType : ObjectType
    {
        public List<DeclarationSyntax> Args { get; private set; }

        public bool Gave { get; set; }
        
        public object GiveVal { get; private set; }

        public FunctionType(string name, object val, List<DeclarationSyntax> args) : base(name, val)
        {
            Args = args;
            Gave = false;
            GiveVal = null;
        }

        public void Give(object val)
        {
            Gave = true;
            GiveVal = val;
        }

        public override string ToString() => $"<function_ '{Name}'>";

        public override ObjectType DeepCopy()
        {
            FunctionType newObj = new FunctionType(this.Name, this.Val, this.Args);
            return newObj;
        }
    }

}
