using Boink.Analysis.Parsing;
using Boink.Analysis.Semantic;
using Boink.Analysis.Semantic.Symbols;
using Boink.Analysis.Tokenization;

using Boink.Text;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Struct to hold information about a package.
    /// </summary>
    public struct PackageInfo : IImportableInfo
    {
        public string Name { get; }

        public string Path { get; }

        public bool IsPackage => true;

        public bool IsLibrary => false;

        /// <summary>
        /// Construct a PackageInfo object.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="packagePath">Path of the package file.</param>
        public PackageInfo(string packageName, string packagePath)
        {
            Name = packageName;
            Path = packagePath;
        }

        public IImportableSymbol ToSymbol(DirectoryCache dirCache)
        {
            // Lex the package.
            var lexer = new Lexer(Path);

            var parser = new Parser(lexer);

            
            // Parse the package.
            var root = parser.Parse(this.Name);

            var symbolTreeBuilder = new SemanticAnalyzer(Path, dirCache);

            // Semantically analyze the package.
            symbolTreeBuilder.Visit(root);

            // Write all logs.
            symbolTreeBuilder.WriteAll();

            // Create a package symbol for the package.
            PackageSymbol symbol = new PackageSymbol(this.Name, root);
            
            // Set the global scope.
            symbol.GlobalScope = symbolTreeBuilder.GlobalScope;

            return symbol;
        }
    }
}
