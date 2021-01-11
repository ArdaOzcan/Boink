using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// A special symbol for libraries.
    /// Includes importables such as other libraries or packages.
    /// </summary>
    public class LibrarySymbol : Symbol, IImportableSymbol
    {
        public bool IsPackage => false;
        
        public bool IsLibrary => true;

        /// <summary>
        /// Symbol table that holds the importable symbols.
        /// <para>
        /// Used to determine what child libraries or
        /// packages this library contains.
        /// </para>
        /// </summary>
        /// <value></value>
        public SymbolTable Importables { get; }

        /// <summary>
        /// Construct a LibrarySymbol object.
        /// </summary>
        /// <param name="name">Name of the symbol.</param>
        /// <param name="importables">Importables that this library contains.</param>
        /// <returns></returns>
        public LibrarySymbol(string name, SymbolTable importables) : base(typeof(LibraryType), name)
        {
            Importables = importables;
        }
    }
}
