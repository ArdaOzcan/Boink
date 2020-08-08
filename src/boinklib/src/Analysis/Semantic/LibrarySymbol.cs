using Boink.AST.Nodes;
using Boink.Types;

namespace Boink.Analysis.Semantic
{
    /// <summary>
    /// Special symbol for libraries.
    /// <para>
    /// Contains the root node.
    /// </para>
    /// </summary>
    public class LibrarySymbol : Symbol
    {
        /// <summary>
        /// Root syntax node of a library.
        /// </summary>
        public ProgramSyntax Root { get; private set; }

        /// <summary>
        /// Global scope of the library.
        /// </summary>
        public SymbolTable GlobalScope { get; set; }

        /// <summary>
        /// Construct a LibrarySymbol object.
        /// </summary>
        /// <param name="name">Name of the library.</param>
        /// <param name="root">Root syntax node of the library.</param>
        public LibrarySymbol(string name, ProgramSyntax root) : base(typeof(lib_), name)
        {
            Root = root;
        }

    }
}
