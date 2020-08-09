using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;

namespace Boink.Types
{

    /// <summary>Base class for all built-in types</summary>
    public class obj_
    {
        public static Dictionary<TokenType, Type> TypeDictionary = new Dictionary<TokenType, Type> 
        { 
            { TokenType.DynamicType, typeof(dyn_) },
            { TokenType.IntType, typeof(int_) },
            { TokenType.BoolType, typeof(bool_) },
            { TokenType.FloatType, typeof(float_) },
            { TokenType.StringType, typeof(string_) }
        };

        public string Name { get; private set; }
        public object Val { get; set; }

        ///<summary>Return corresponding type class with a given token type.</summary>
        ///<param name="tokenType">Given token type.</param>
        ///<returns>Corresponding type class.</returns>
        public static Type GetTypeByTokenType(TokenType tokenType) => TypeDictionary[tokenType];

        public override string ToString() => $"{Val}";

        public virtual obj_ DeepCopy()
        {
            obj_ newObj = new obj_(this.Name, this.Val);
            return newObj;
        }

        ///<summary>Construct a 'obj_' object with a variable name and value.</summary>
        ///<param name="name">Name of the variable.</param>
        ///<param name="val">Value of the variable.</param>
        public obj_(string name, object val)
        {
            Name = name;
            Val = val;
        }

        public virtual object add(obj_ other) => throw new NotImplementedException();

        public virtual object subtract(obj_ other) => throw new NotImplementedException();

        public virtual object divide(obj_ other) => throw new NotImplementedException();

        public virtual object multiply(obj_ other) => throw new NotImplementedException();

        public virtual object and(obj_ other) => throw new NotImplementedException();

        public virtual object or(obj_ other) => throw new NotImplementedException();

        public virtual object greater(obj_ other) => throw new NotImplementedException();

        public virtual object less(obj_ other) => throw new NotImplementedException();

        public virtual object greaterEquals(obj_ other) => throw new NotImplementedException();

        public virtual object lessEquals(obj_ other) => throw new NotImplementedException();

        public virtual object equalsEquals(obj_ other)
        {
            bool_ var = new bool_(null, this.Val.Equals(other.Val));
            return var;
        }

        public virtual object negative() => null;

        public virtual object positive() => null;

    }

}
