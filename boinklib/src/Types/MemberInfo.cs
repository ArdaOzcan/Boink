using Boink.Interpretation;

namespace Boink.Types
{
    public sealed class MemberData {
        public MemberData(ClassType type, ActivationRecord attributes)
        {
            Type = type;
            Attributes = attributes;
        }

        public MemberData(ClassType type)
        {
            Type = type;
            Attributes = null;
        }

        public ClassType Type { get; }
        public ActivationRecord Attributes { get; internal set; }
    }
}