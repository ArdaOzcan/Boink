using System;

namespace Boink.Analysis.Semantic
{
    /// <summary>
    /// Symbol for variables. Same as the base class.
    /// </summary>
    public class VarSymbol : Symbol
    {
        public VarSymbol(Type varType, string name) : base(varType, name)
        {

        }
    }
}
