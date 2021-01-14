
using Boink.AST.Nodes;

using Boink.Types;

namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// Special symbol for packages.
    /// <para>
    /// Contains the root node.
    /// </para>
    /// /// </summary>
    public class PackageSymbol : Symbol, IImportableSymbol
    {
        public bool IsPackage { get => true; }
        
        public bool IsLibrary { get => false; }

        /// <summary>
        /// Root syntax node of a package.
        /// </summary>
        public ProgramSyntax Root { get; private set; }

        /// <summary>
        /// Global scope of the package.
        /// </summary>
        public SymbolTable GlobalScope { get; set; }

        /// <summary>
        /// Construct a LibrarySymbol object.
        /// </summary>
        /// <param name="name">Name of the package.</param>
        /// <param name="root">Root syntax node of the package.</param>
        public PackageSymbol(string name, ProgramSyntax root) : base(new BoinkType(typeof(PackageType)), name)
        {
            Root = root;
        }

    }
}
