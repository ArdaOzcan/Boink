using System;

namespace Boink.Types
{
    public sealed class float_ : obj_
    {
        public float_(string name, object val) : base(name, val) { }

        public override object add(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new float_(null, (float)Val + (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (float)Val + (float)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new float_(null, (float)Val - (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (float)Val - (float)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object multiply(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new float_(null, (float)Val * (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (float)Val * (float)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object divide(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new float_(null, (float)Val * (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (float)Val * (float)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object less(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new bool_(null, (float)Val < (int)other.Val);
            if (otherType == typeof(float_))
                return new bool_(null, (float)Val < (float)other.Val);


            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new float_(null, +(float)Val);
        }

        public override object negative()
        {
            return new float_(null, -(float)Val);
        }

        public override obj_ DeepCopy()
        {
            float_ newObj = new float_(Name, Val);
            return newObj;
        }
    }

}
