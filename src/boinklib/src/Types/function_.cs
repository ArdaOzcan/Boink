using System.Collections.Generic;

using Boink.AST;
using Boink.AST.Nodes;

namespace Boink.Types
{
    public sealed class function_ : obj_
    {
        public List<DeclarationSyntax> Args { get; private set; }

        public bool Gave { get; set; }
        
        public object GiveVal { get; private set; }

        public function_(string name, object val, List<DeclarationSyntax> args) : base(name, val)
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

        public override obj_ DeepCopy()
        {
            function_ newObj = new function_(this.Name, this.Val, this.Args);
            return newObj;
        }
    }

}
