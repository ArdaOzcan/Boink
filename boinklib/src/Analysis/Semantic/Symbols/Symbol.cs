using System;
using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// Base class for symbols.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Type of the variable.
        /// </summary>
        public BoinkType VarType { get; protected set; }

        /// <summary>
        /// Name of the variable.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Construct a Symbol object.
        /// </summary>
        /// <param name="varType">Type of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        public Symbol(BoinkType varType, string name)
        {
            VarType = varType;
            Name = name;
        }

        public override string ToString() => $"{VarType}: {Name}";
    }
}
