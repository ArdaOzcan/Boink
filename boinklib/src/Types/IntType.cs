using System;

namespace Boink.Types
{

    public sealed class IntType : ObjectType
    {
        public IntType(string name, object val) : base(name, val)
        {

        }

        public override object multiply(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new IntType(null, (int)Val * (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (int)Val * (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (int)Val * (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object divide(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new DoubleType(null, (int)Val / (double)(int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (int)Val / (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (int)Val / (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object add(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new IntType(null, (int)Val + (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (int)Val + (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (int)Val + (double)other.Val);

            throw new Exception("Shouldn't have come here");
        }

        public override object less(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new BoolType(null, (int)Val < (int)other.Val);
            if (otherType == typeof(FloatType))
                return new BoolType(null, (int)Val < (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new BoolType(null, (int)Val < (double)other.Val);


            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(ObjectType other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(IntType))
                return new IntType(null, (int)Val - (int)other.Val);
            if (otherType == typeof(FloatType))
                return new FloatType(null, (int)Val - (float)other.Val);
            if (otherType == typeof(DoubleType))
                return new DoubleType(null, (int)Val - (double)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new IntType(null, +(int)Val);
        }

        public override object negative()
        {
            return new IntType(null, -(int)Val);
        }

        public override ObjectType DeepCopy()
        {
            IntType newObj = new IntType(Name, Val);
            return newObj;
        }
    }

}
