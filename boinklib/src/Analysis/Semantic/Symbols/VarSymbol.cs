using System;
using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// Symbol for variables. Same as the base class.
    /// </summary>
    public class VarSymbol : Symbol
    {
        public VarSymbol(BoinkType varType, string name) : base(varType, name)
        {

        }
    }
}
