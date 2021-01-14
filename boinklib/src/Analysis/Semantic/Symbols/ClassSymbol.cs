using System;
using System.Collections.Generic;

using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// Special symbol for functions.
    /// </summary>
    public sealed class ClassSymbol : Symbol
    {
        public SymbolTable SymbolTable { get; set; }
        public FunctionSymbol Constructor { get; internal set; }

        public ClassSymbol(string name) : base(null, name)
        {
            BoinkType.AddUserDefinedType(name);
            VarType = new BoinkType(BoinkType.userTypes[name]);
        }


        public override string ToString() => $"{VarType} Type";


        public ClassInfo ToClassInfo()
        {

            var c = new ClassInfo(Name);

            foreach (var kv in SymbolTable)
            {
                var varName = kv.Key;
                var symbol = kv.Value;

                if (symbol.GetType() == typeof(FunctionSymbol))
                    c.Methods.Add((FunctionSymbol)symbol);
                else if (symbol.GetType() == typeof(VarSymbol))
                    c.Fields.Add((VarSymbol)symbol);
            }

            return c;
        }
    }
}
