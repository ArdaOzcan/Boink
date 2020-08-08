using System;

namespace Boink.Analysis.Semantic
{

    /// <summary>
    /// Base class for symbols.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Type of the variable.
        /// </summary>
        public Type VarType { get; private set; }

        /// <summary>
        /// Name of the variable.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Construct a Symbol object.
        /// </summary>
        /// <param name="varType">Type of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        public Symbol(Type varType, string name)
        {
            VarType = varType;
            Name = name;
        }

        public override string ToString() => $"{VarType}: {Name}";
    }
}
