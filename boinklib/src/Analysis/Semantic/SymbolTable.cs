using System.Collections.Generic;

using Boink.Analysis.Semantic.Symbols;

namespace Boink.Analysis.Semantic
{

    /// <summary>
    /// A symbol table that holds symbols for a certain scope.
    /// </summary>
    public class SymbolTable : Dictionary<string, Symbol>
    {
        /// <summary>
        /// Name of the scope.
        /// </summary>
        public string ScopeName { get; private set; }

        /// <summary>
        /// Owner function symbol of this symbol table is there is any.
        /// </summary>
        public Symbol Owner { get; set; }

        /// <summary>
        /// Parent symbol table of this symbol table.
        /// </summary>
        public SymbolTable Parent { get; private set; }

        /// <summary>
        /// Construct a SymbolTable object.
        /// </summary>
        /// <param name="scopeName">Name of the scope</param>
        /// <param name="owner">Owner of this symbol table, a function symbol. Defaults to null.</param>
        /// <param name="parentScope">Parent SymbolTable to check outer scopes. Defaults to null.</param>
        public SymbolTable(string scopeName, Symbol owner = null, SymbolTable parentScope = null) : base()
        {
            ScopeName = scopeName;
            Owner = owner;
            Parent = parentScope;
        }

        /// <summary>
        /// Define a new symbol in the symbol table.
        /// </summary>
        /// <param name="symbol">Symbol to be defined.</param>
        public void Define(Symbol symbol) 
        {
            if(symbol.Name != null)
                this[symbol.Name] = symbol;
        }

        /// <summary>
        /// Check the current scope and then the outer scopes.
        /// If the name is not defined, return null, otherwise
        /// return the symbol. 
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <returns>Corresponding symbol or null.</returns>
        public Symbol Lookup(string name)
        {
            Symbol symbol = null;
            bool present = TryGetValue(name, out symbol);
            
            if (!present && Parent != null) symbol = Parent.Lookup(name);

            return symbol;
        }

        /// <summary>
        /// Check only the current scope for the name.
        /// Return null if the name doesn't exists.
        /// <para>
        /// Used for overriding outer definitions. 
        /// </para>
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <returns>Corresponding symbol or null.</returns>
        public Symbol LookupOnlyCurrentScope(string name)
        {
            if(name == null) return null;

            Symbol symbol;
            if (TryGetValue(name, out symbol)) return symbol;

            return null;
        }

    }
}
