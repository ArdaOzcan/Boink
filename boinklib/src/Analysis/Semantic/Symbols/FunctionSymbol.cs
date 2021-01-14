using System;
using System.Collections.Generic;

using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// Special symbol for functions.
    /// </summary>
    public sealed class FunctionSymbol : Symbol
    {
        /// <summary>
        /// List of the types of arguments.
        /// </summary>
        public List<BoinkType> ArgTypes { get; private set; }

        /// <summary>
        /// Type of the give expression if any.
        /// </summary>
        public BoinkType GiveType { get; set; }

        /// <summary>
        /// Construct a FunctionSymbol object.
        /// </summary>
        /// <param name="argTypes">List of argument types for semantic analysis.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="giveType">Given type from the function.</param>
        public FunctionSymbol(List<BoinkType> argTypes, string name, BoinkType giveType) : base(new BoinkType(typeof(FunctionType)), name)
        {
            ArgTypes = argTypes;
            GiveType = giveType;
        }

        public override string ToString() => $"{VarType} {string.Join(", ", ArgTypes)} {Name}";
    }
}
