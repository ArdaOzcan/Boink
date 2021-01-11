using System.Collections.Generic;
using System.Reflection;

namespace Boink.Types
{
    public sealed class StringType : ObjectType
    {
        public static Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>
        {
            {"subString", typeof(StringType).GetMethod("SubString")}
        };

        public static StringType SubString(object o, IntType start, IntType end)
        {
            return new StringType(null, ((string)((StringType)o).Val).Substring((int)start.Val, (int)end.Val));
        }

        public StringType(string name, object val) : base(name, val)
        {

        }

        public override object add(ObjectType other)
        {
            return new StringType(null, (string)Val + (string)other.Val);
        }

        public override ObjectType DeepCopy()
        {
            StringType newObj = new StringType(this.Name, this.Val);
            return newObj;
        }
    }

}
