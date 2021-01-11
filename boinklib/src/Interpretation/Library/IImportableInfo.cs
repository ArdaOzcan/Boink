using Boink.Analysis.Semantic.Symbols;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Interface for importable information such as 
    /// library information or package information.
    /// </summary>
    public interface IImportableInfo
    {
        /// <summary>
        /// Name of the importable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Path of the importable, a file if it's a package, 
        /// a directory if it's a library.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Boolean to determine if this importable is a package.
        /// </summary>
        bool IsPackage { get; }
        
        /// <summary>
        /// Boolean to determine if this importable is a package.
        /// </summary>
        bool IsLibrary { get; }

        /// <summary>
        /// Convert an importable info to a symbol with the given directory cache.
        /// </summary>
        /// <param name="dirCache">Given directory cache.</param>
        /// <returns>An importable symbol.</returns>
        IImportableSymbol ToSymbol(DirectoryCache dirCache);
    }
}
