using System;

namespace Boink.Types
{
    public sealed class double_ : obj_
    {
        public double_(string name, object val) : base(name, val) { }

        public override object add(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new double_(null, (double)Val + (int)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (double)Val + (double)other.Val);
            if (otherType == typeof(float_))
                return new double_(null, (double)Val + (float)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object subtract(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new double_(null, (double)Val - (int)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (double)Val - (double)other.Val);
            if (otherType == typeof(float_))
                return new double_(null, (double)Val - (float)other.Val);

            throw new Exception("Shouldn't have come here");
        }

        public override object multiply(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new double_(null, (double)Val * (int)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (double)Val * (double)other.Val);
            if (otherType == typeof(float_))
                return new double_(null, (double)Val * (float)other.Val);
                

            throw new Exception("Shouldn't have come here");
        }

        public override object divide(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new double_(null, (double)Val * (int)other.Val);
            if (otherType == typeof(double_))
                return new double_(null, (double)Val * (double)other.Val);
                
            throw new Exception("Shouldn't have come here");
        }

        public override object less(obj_ other)
        {
            Type otherType = other.GetType();
            if (otherType == typeof(int_))
                return new bool_(null, (double)Val < (int)other.Val);
            if (otherType == typeof(double_))
                return new bool_(null, (double)Val < (double)other.Val);
            if (otherType == typeof(float_))
                return new double_(null, (double)Val < (float)other.Val);
                


            throw new Exception("Shouldn't have come here");
        }

        public override object positive()
        {
            return new double_(null, +(double)Val);
        }

        public override object negative()
        {
            return new double_(null, -(double)Val);
        }

        public override obj_ DeepCopy()
        {
            double_ newObj = new double_(Name, Val);
            return newObj;
        }
    }

}
