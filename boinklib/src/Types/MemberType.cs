using System;
using System.Collections.Generic;
using Boink.AST.Nodes;
using Boink.Interpretation;
using Boink.Interpretation.Library;

namespace Boink.Types
{
    public sealed class MemberType : ObjectType
    {
        private ActivationRecord activationRecord;
        readonly ClassType memberOf;

        public ActivationRecord AR { get => activationRecord; }

        public MemberType(string name, object val) : base(name, val)
        {
            memberOf = (ClassType)val;
            activationRecord = new ActivationRecord(
                            name: Name,
                            nestingLevel: 1,
                            parentRecord: memberOf.ParentRecord
                        );
            activationRecord.DefineVar("self", this);
            Construct();
        }

        public static MemberType Assign(string name, MemberType other)
        {
            var thisObj = new MemberType(name, other.memberOf);

            thisObj.activationRecord.AssignMembers(other.activationRecord.Members);
            return thisObj;
        }

        public void Construct()
        {
            var interpreter = new Interpretation.Interpreter(null, new DirectoryCache());
            interpreter.ProgramCallStack.Push(AR);
            foreach (var statement in (List<SyntaxNode>)memberOf.Val)
            {
                interpreter.Visit(statement);
            }
        }

        public override string ToString() => $"<{memberOf.Name} '{Name}'>";

        public override ObjectType DeepCopy()
        {
            MemberType newObj = new MemberType(this.Name, this.Val);
            return newObj;
        }
    }
}
