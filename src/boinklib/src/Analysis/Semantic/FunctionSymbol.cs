using System;
using System.Collections.Generic;

using Boink.Types;

namespace Boink.Analysis.Semantic
{
    /// <summary>
    /// Special symbol for functions.
    /// </summary>
    public sealed class FunctionSymbol : Symbol
    {
        /// <summary>
        /// List of the types of arguments.
        /// </summary>
        public List<Type> ArgTypes { get; private set; }

        /// <summary>
        /// Type of the give expression if any.
        /// </summary>
        public Type GiveType { get; private set; }

        /// <summary>
        /// Construct a FunctionSymbol object.
        /// </summary>
        /// <param name="argTypes">List of argument types for semantic analysis.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="giveType">Given type from the function.</param>
        public FunctionSymbol(List<Type> argTypes, string name, Type giveType) : base(typeof(function_), name)
        {
            ArgTypes = argTypes;
            GiveType = giveType;
        }

        public override string ToString() => $"{VarType} {string.Join(", ", ArgTypes)} {Name}";
    }
}
