using System.Collections.Generic;
using Boink.Analysis.Semantic.Symbols;

namespace Boink.Types
{
    public class ClassInfo
    {
        public HashSet<FunctionSymbol> Methods { get; set; }
        public HashSet<VarSymbol> Fields { get; set; }
        public string Name { get; }

        public ClassInfo(string name)
        {
            Name = name;

            Methods = new HashSet<FunctionSymbol>();
            Fields = new HashSet<VarSymbol>();
        }
    }
}