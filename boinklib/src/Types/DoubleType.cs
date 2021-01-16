using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boink.Types
{
    public sealed class DoubleType : ObjectType
    {
        public static Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>
        {
            {"toString", typeof(DoubleType).GetMethod("DoubleToStringMethod")},
            {"floor", typeof(DoubleType).GetMethod("FloorMethod")},
        };

        public static StringType DoubleToStringMethod(object o)
        {
            return new StringType(null, ((double)((DoubleType)o).Val).ToString());
        }

        public static DoubleType FloorMethod(object o)
        {
            return new DoubleType(null, Math.Floor((double)((DoubleType)o).Val));
        }

        public DoubleType(string name, object val) : base(name, val) { }

        public override object add(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new DoubleType(null, (double)Val + (int)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (double)Val + (double)other.Val);
            if (otherType == typeof(FloatType))
                return new DoubleType(null, (double)Val + (float)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new DoubleType(null, (double)Val - (int)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (double)Val - (double)other.Val);
            if (otherType == typeof(FloatType))
                return new DoubleType(null, (double)Val - (float)other.Val);

            throw new Exception("Shouldn't have come here");
        }

        public override object multiply(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new DoubleType(null, (double)Val * (int)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (double)Val * (double)other.Val);
            if (otherType == typeof(FloatType))
                return new DoubleType(null, (double)Val * (float)other.Val);
                

            throw new Exception("Shouldn't have come here");
        }

        public override object divide(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new DoubleType(null, (double)Val * (int)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (double)Val * (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object less(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new BoolType(null, (double)Val < (int)other.Val);
            if (otherType == typeof(DoubleType))
                return new BoolType(null, (double)Val < (double)other.Val);
            if (otherType == typeof(FloatType))
                return new DoubleType(null, (double)Val < (float)other.Val);
                


            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new DoubleType(null, +(double)Val);
        }

        public override object negative()
        {
            return new DoubleType(null, -(double)Val);
        }

        public override ObjectType DeepCopy()
        {
            DoubleType newObj = new DoubleType(Name, Val);
            return newObj;
        }
    }

}
