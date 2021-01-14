using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;

namespace Boink.Types
{

    /// <summary>Base class for all built-in types</summary>
    public class ObjectType
    {
        public static Dictionary<TokenType, Type> TypeDictionary = new Dictionary<TokenType, Type> 
        { 
            { TokenType.IntType, typeof(IntType) },
            { TokenType.BoolType, typeof(BoolType) },
            { TokenType.FloatType, typeof(FloatType) },
            { TokenType.StringType, typeof(StringType) },
            { TokenType.DoubleType, typeof(DoubleType) }
        };

        public string Name { get; private set; }
        public object Val { get; set; }

        ///<summary>Return corresponding type class with a given token type.</summary>
        ///<param name="tokenType">Given token type.</param>
        ///<returns>Corresponding type class.</returns>
        public static Type GetTypeByTokenType(TokenType tokenType) 
        {
            Type val;
            TypeDictionary.TryGetValue(tokenType, out val);

            return val;
        }

        public override string ToString() => $"{Val}";

        public virtual ObjectType DeepCopy()
        {
            ObjectType newObj = new ObjectType(this.Name, this.Val);
            return newObj;
        }

        ///<summary>Construct a 'obj_' object with a variable name and value.</summary>
        ///<param name="name">Name of the variable.</param>
        ///<param name="val">Value of the variable.</param>
        public ObjectType(string name, object val)
        {
            Name = name;
            Val = val;
        }

        public virtual object add(ObjectType other) => throw new NotImplementedException();

        public virtual object subtract(ObjectType other) => throw new NotImplementedException();

        public virtual object divide(ObjectType other) => throw new NotImplementedException();

        public virtual object multiply(ObjectType other) => throw new NotImplementedException();

        public virtual object and(ObjectType other) => throw new NotImplementedException();

        public virtual object or(ObjectType other) => throw new NotImplementedException();

        public virtual object greater(ObjectType other) => throw new NotImplementedException();

        public virtual object less(ObjectType other) => throw new NotImplementedException();

        public virtual object greaterEquals(ObjectType other) => throw new NotImplementedException();

        public virtual object lessEquals(ObjectType other) => throw new NotImplementedException();

        public virtual object equalsEquals(ObjectType other)
        {
            BoolType var = new BoolType(null, this.Val.Equals(other.Val));
            return var;
        }

        public virtual object negative() => null;

        public virtual object positive() => null;

    }

}
