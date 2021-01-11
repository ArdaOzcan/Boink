using System.Collections.Generic;

using Boink.Analysis.Semantic;

using Boink.Analysis.Semantic.Symbols;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Struct to hold information about a library.
    /// </summary>
    public struct LibraryInfo : IImportableInfo
    {
        /// <summary>
        /// Importables such as libraries or packages that this library contains.
        /// </summary>
        public Dictionary<string, IImportableInfo> Importables { get; }

        public string Name { get; }

        public string Path { get; }

        public bool IsPackage => false;

        public bool IsLibrary => true;

        /// <summary>
        /// Construct a LibraryInfo object.
        /// </summary>
        /// <param name="libraryName">Name of the library.</param>
        /// <param name="libraryPath">Path of the library directory.</param>
        /// <param name="importables">Importables of the library.</param>
        public LibraryInfo(string libraryName, string libraryPath, Dictionary<string, IImportableInfo> importables)
        {
            Name = libraryName;
            Path = libraryPath;
            Importables = importables;
        }

        public IImportableSymbol ToSymbol(DirectoryCache dirCache)
        {
            var symbols = new SymbolTable(Name);
            foreach(var res in Importables)
                symbols.Add(res.Key, (Symbol)res.Value.ToSymbol(dirCache));

            return new LibrarySymbol(Name, symbols);
        }
    }
}
