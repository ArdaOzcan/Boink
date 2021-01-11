using System;

namespace Boink.Types
{

    public sealed class int_ : obj_
    {
        public int_(string name, object val) : base(name, val)
        {

        }

        public override object multiply(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new int_(null, (int)Val * (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (int)Val * (float)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (int)Val * (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object divide(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new double_(null, (int)Val / (double)(int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (int)Val / (float)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (int)Val / (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object add(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new int_(null, (int)Val + (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (int)Val + (float)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (int)Val + (double)other.Val);

            throw new Exception("Shouldn't have come here");
        }

        public override object less(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new bool_(null, (int)Val < (int)other.Val);
            if (otherType == typeof(float_))
                return new bool_(null, (int)Val < (float)other.Val);
            if (otherType == typeof(double_))
                return new bool_(null, (int)Val < (double)other.Val);


            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new int_(null, (int)Val - (int)other.Val);
            if (otherType == typeof(float_))
                return new float_(null, (int)Val - (float)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (int)Val - (double)other.Val);
            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new int_(null, +(int)Val);
        }

        public override object negative()
        {
            return new int_(null, -(int)Val);
        }

        public override obj_ DeepCopy()
        {
            int_ newObj = new int_(Name, Val);
            return newObj;
        }
    }

}
