using System.Collections.Generic;
using System.Reflection;

namespace Boink.Types
{
    public sealed class StringType : ObjectType
    {
        public static Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>
        {
            {"subString", typeof(StringType).GetMethod("SubString")},
            {"toInt", typeof(StringType).GetMethod("StringToInt")},
            {"split", typeof(StringType).GetMethod("StringSplit")}
        };

        public static StringType StringSplit(object o, StringType delimeter, IntType index)
        {
            string val = (string)((StringType)o).Val;
            string delimeterStr = (string)delimeter.Val;
            string returnVal = val.Split(delimeterStr.ToCharArray())[(int)index.Val];

            return new StringType(null, returnVal);
        }

        public static StringType SubString(object o, IntType start, IntType end)
        {
            return new StringType(null, ((string)((StringType)o).Val).Substring((int)start.Val, (int)end.Val));
        }

        public static IntType StringToInt(object o)
        {
            return new IntType(null, int.Parse((string)((StringType)o).Val));
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
