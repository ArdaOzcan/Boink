using System;
using System.Collections.Generic;
using Boink.Analysis.Semantic.Symbols;

namespace Boink.Types
{
    public class BoinkType
    {
        static public Dictionary<string, ClassInfo> userTypes = new Dictionary<string, ClassInfo>();

        public string Name { get; set; }
        public bool IsBuiltin { get; }
        public bool IsUserDefined { get => !IsBuiltin; }
        public Type CSType { get; }
        public ClassInfo UType { get; }

        public BoinkType() 
        {
        }

        public BoinkType(Type type) 
        {
            IsBuiltin = true;
            CSType = type;
            if(type != null)
                Name = type.Name;
        }

        public BoinkType(ClassInfo type) 
        {
            IsBuiltin = false;
            UType = type;
            if(type != null)
                Name = type.Name;
        }

        public static void AddUserDefinedType(string name)
        {
            userTypes.Add(name, new ClassInfo(name));
        }

        public bool IsEqual(Type other)
        {
            return IsBuiltin && CSType == other;
        }

        public bool IsEqual(ClassInfo other)
        {
            return !IsBuiltin && UType == other;
        }

        public bool IsEqual(BoinkType other)
        {
            return (!IsBuiltin && UType == other.UType) || (IsBuiltin && CSType == other.CSType);
        }

        public override string ToString() => Name;
    }
}