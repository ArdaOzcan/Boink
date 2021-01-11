using System;

namespace Boink.Types
{
    public sealed class FloatType : ObjectType
    {
        public FloatType(string name, object val) : base(name, val) { }

        public override object add(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new FloatType(null, (float)Val + (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (float)Val + (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (float)Val + (double)other.Val);

            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new FloatType(null, (float)Val - (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (float)Val - (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (float)Val - (double)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object multiply(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new FloatType(null, (float)Val * (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (float)Val * (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (float)Val * (double)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object divide(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new FloatType(null, (float)Val * (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (float)Val * (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (float)Val / (double)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object less(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new BoolType(null, (float)Val < (int)other.Val);
            if (otherType == typeof(FloatType))
                return new BoolType(null, (float)Val < (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new BoolType(null, (float)Val < (double)other.Val);


            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new FloatType(null, +(float)Val);
        }

        public override object negative()
        {
            return new FloatType(null, -(float)Val);
        }

        public override ObjectType DeepCopy()
        {
            FloatType newObj = new FloatType(Name, Val);
            return newObj;
        }
    }

}
