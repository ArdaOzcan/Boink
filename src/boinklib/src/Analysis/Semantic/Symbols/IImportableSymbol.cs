namespace Boink.Analysis.Semantic.Symbols
{
    /// <summary>
    /// An interface for directly importable symbols such as libraries and packages.
    /// <para>
    /// Other (relatively importable) symbols such as functions and variables are
    /// imported through the 'from' keyword (in futher versions).
    /// </para>
    /// </summary>
    public interface IImportableSymbol
    {
        /// <summary>
        /// Boolean to determine if this importable symbol is a package.
        /// </summary>
        bool IsPackage { get; }
        
        /// <summary>
        /// Boolean to determine if this importable symbol is a library.
        /// </summary>
        bool IsLibrary { get; }
    }
}
