using System;
using System.Collections.Generic;
using Boink.AST.Nodes;
using Boink.Interpretation;
using Boink.Interpretation.Library;

namespace Boink.Types
{
    public sealed class MemberType : ObjectType
    {

        public ActivationRecord AR { get => ((MemberData)Val).Attributes; }
        public ClassType MemberOf { get; }

        public MemberType(string name, object val) : base(name, val)
        {
            var memberInfo = (MemberData)val;
            MemberOf = memberInfo.Type;
            ActivationRecord activationRecord;
            activationRecord = new ActivationRecord(name: Name,
                                nestingLevel: 1,
                                parentRecord: MemberOf.ParentRecord);
            if (memberInfo.Attributes == null)
            {
                activationRecord.DefineVar("self", this);
                memberInfo.Attributes = activationRecord;
                Construct();
            }
            else
            {
                foreach (var item in memberInfo.Attributes.Members)
                {
                    if (item.Key.Equals("self"))
                    {
                        activationRecord.DefineVar(item.Key, this);
                        continue;
                    }
                    activationRecord.DefineVar(item.Key, item.Value);
                }
                memberInfo.Attributes = activationRecord;
            }
        }

        public static MemberType Assign(string name, MemberType other)
        {
            var thisObj = new MemberType(name, other.Val);
            return thisObj;
        }

        public void Construct()
        {
            var interpreter = new Interpretation.Interpreter(null, new DirectoryCache());
            interpreter.ProgramCallStack.Push(AR);
            foreach (var statement in (List<SyntaxNode>)MemberOf.Val)
            {
                interpreter.Visit(statement);
            }
        }

        public override string ToString() => $"<{MemberOf.Name} '{Name}'>";

        public override ObjectType DeepCopy()
        {
            MemberType newObj = new MemberType(this.Name, this.Val);
            return newObj;
        }
    }
}
