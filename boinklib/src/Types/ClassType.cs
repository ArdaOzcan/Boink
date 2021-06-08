using System;
using System.Collections.Generic;
using Boink.AST.Nodes;
using Boink.Interpretation;
using Boink.Interpretation.Library;

namespace Boink.Types
{
    public sealed class ClassType : ObjectType
    {
        public ActivationRecord ParentRecord { get; set; }
        public ClassType(string name, object val) : base(name, val)
        {
        }

        public override string ToString() => $"<type '{Name}'>";

        public override ObjectType DeepCopy()
        {
            ClassType newObj = new ClassType(this.Name, this.Val);
            return newObj;
        }
    }
}
