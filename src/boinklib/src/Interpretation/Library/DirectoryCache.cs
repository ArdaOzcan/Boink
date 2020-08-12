using System.Collections;
using System.Collections.Generic;
using System.IO;

using Boink.Analysis.Parsing;
using Boink.Analysis.Semantic;
using Boink.Analysis.Semantic.Symbols;
using Boink.Analysis.Tokenization;
using Boink.Text;

using Boink.Types;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Struct to contain information about libraries and packages that are inside a directory and
    /// convert them to symbols/objects.
    /// </summary>
    public struct DirectoryCache
    {
        /// <summary>
        /// Actual field that contains the information about the directory.
        /// </summary>
        Dictionary<string, IImportableInfo> cache;

        /// <summary>
        /// Construct a DirectoryCache object.
        /// </summary>
        /// <param name="rootDirPath">Path of the root directory.</param>
        public DirectoryCache(string rootDirPath)
        {
            cache = GetLibraryResources(rootDirPath);
        }

        /// <summary>
        /// Recursively create and return information about every package and library inside a directory.
        /// </summary>
        /// <param name="dirPath">Path of the directory.</param>
        /// <returns>The cache as a dictionary.</returns>
        private static Dictionary<string, IImportableInfo> GetLibraryResources(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            var resources = new Dictionary<string, IImportableInfo>();
            foreach (string filePath in files)
            {
                string packageName = Path.GetFileNameWithoutExtension(filePath);
                var package = new PackageInfo(packageName, filePath);
                resources.Add(packageName, package);
            }

            string[] directories = Directory.GetDirectories(dirPath);
            foreach (string path in directories)
            {
                string libraryName = new DirectoryInfo(path).Name;
                var libResources = GetLibraryResources(path);
                var lib = new LibraryInfo(libraryName, path, libResources);
                resources.Add(libraryName, lib);
            }

            return resources;
        }

        /// <summary>
        /// Enumerable method to yield importable information about a package/library
        /// in the hierarchy string.
        /// </summary>
        /// <param name="hierarchy">A list of strings which represents the hierarchical order of the package/library</param>
        /// <returns>Importable information.</returns>
        public IEnumerable GetImportableInfos(List<string> hierarchy)
        {
            IImportableInfo currentImportable = null;
            var currentCache = cache;
            for (int i = 0; i < hierarchy.Count; i++)
            {
                string s = hierarchy[i];
                if (currentCache.TryGetValue(s, out currentImportable))
                {
                    if (currentImportable.IsLibrary)
                    {
                        LibraryInfo libInfo = ((LibraryInfo)currentImportable);
                        currentCache = libInfo.Importables;
                        
                        yield return libInfo;
                    }
                    else if (currentImportable.IsPackage)
                        yield return (PackageInfo)currentImportable;      
                }
            }
        }

        /// <summary>
        /// Check if such a library/package exists with the hierarchical order given.
        /// <para>
        /// Note: Planned to be removed in future versions.
        /// </para>
        /// </summary>
        /// <param name="hierarchy">A list of strings which represents the hierarchical order of the package/library</param>
        /// <returns>If the library/package exists</returns>
        public bool HasLibraryOrPackage(List<string> hierarchy)
        {
            IImportableInfo currentImportable;
            var currentCache = cache;
            foreach (string s in hierarchy)
            {
                if (currentCache.TryGetValue(s, out currentImportable))
                {
                    if (currentImportable.IsLibrary)
                        currentCache = ((LibraryInfo)currentImportable).Importables;
                }
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Return importable information about a library/package that has the given hierarchy.
        /// <para>
        /// This method returns the last component of the hierarchy so a hierarchy 'math.geometry' would 
        /// return information about the geometry package/library.
        /// </para>
        /// </summary>
        /// <param name="hierarchy">A list of strings which represents the hierarchical order of the package/library</param>
        /// <returns>Importable information about a library/package.</returns>
        public IImportableInfo GetLibraryOrPackage(List<string> hierarchy)
        {
            IImportableInfo currentImportable = null;
            var currentCache = cache;
            foreach (string s in hierarchy)
            {
                if (currentCache.TryGetValue(s, out currentImportable))
                {
                    if (currentImportable.IsLibrary)
                        currentCache = ((LibraryInfo)currentImportable).Importables;
                }
            }

            return currentImportable;
        }

        /// <summary>
        /// Return the symbol of an importable with the given hierarchy.
        /// </summary>
        /// <param name="hierarchy">A list of strings which represents the hierarchical order of the package/library</param>
        /// <returns>Symbol of a library/package.</returns>
        public IImportableSymbol GetLibraryOrPackageSymbol(List<string> hierarchy)
        {
            IImportableSymbol firstSymbol = null;
            IImportableSymbol currentSymbol = null;

            int pos = 0;
            foreach(IImportableInfo info in GetImportableInfos(hierarchy))
            {
                var dirCache = new DirectoryCache(Path.GetDirectoryName(info.Path));
                if(info.IsLibrary)
                {
                    LibrarySymbol lib;
                    if (pos == hierarchy.Count - 1)
                        lib = (LibrarySymbol)info.ToSymbol(dirCache);
                    else
                    {
                        var symbols = new SymbolTable(info.Name);
                        lib = new LibrarySymbol(info.Name, symbols);
                    }

                    currentSymbol = lib;
                }   
                else
                {
                    PackageSymbol package = (PackageSymbol)info.ToSymbol(dirCache);
                    if (currentSymbol != null && currentSymbol.IsLibrary)
                        ((LibrarySymbol)currentSymbol).Importables.Define(package);

                    currentSymbol = package;
                }

                if (firstSymbol == null)
                        firstSymbol = currentSymbol;

                ++pos;
            }

            return firstSymbol;
        }

        /// <summary>
        /// Create a new package_ object with a given package information
        /// and return it.
        /// <para>
        /// This method reads the package file, lexes, parses and interprets it 
        /// before returning to the caller.
        /// </para>
        /// </summary>
        /// <param name="packageInfo">Information of the package.</param>
        /// <returns>A package_ object.</returns>
        package_ ConvertToPackageObject(PackageInfo packageInfo)
        {
            string text = TextOperations.ReadFileNormalized(packageInfo.Path);
            var lexer = new Lexer(text);
            var parser = new Parser(lexer);

            string fileName = Path.GetFileName(packageInfo.Path);
            var root = parser.Parse(fileName);

            string programDirectory = Path.GetDirectoryName(packageInfo.Path);

            string rootDirPath = Path.GetDirectoryName(packageInfo.Path);
            DirectoryCache dirCache = new DirectoryCache(rootDirPath);
            var interpreter = new Interpreter(programDirectory, dirCache);

            var libRecord = interpreter.Interpret(root);

            return new package_(packageInfo.Name, libRecord);
        }

        /// <summary>
        /// Create a new lib_ object with a given library information
        /// and return it.
        /// </summary>
        /// <param name="libraryInfo">Information of the library.</param>
        /// <returns>A lib_ object.</returns>
        lib_ ConvertToLibraryObject(LibraryInfo libraryInfo)
        {
            var libRecord = new ActivationRecord(libraryInfo.Name, 0, null);
            foreach (IImportableInfo info in libraryInfo.Importables.Values)
            {
                if (info.IsPackage)
                    libRecord[info.Name] = ConvertToPackageObject((PackageInfo)info);
                else if (info.IsLibrary)
                    libRecord[info.Name] = ConvertToLibraryObject((LibraryInfo)info);
            }

            return new lib_(libraryInfo.Name, libRecord);
        }

        /// <summary>
        /// Return the corresponding object to a library/package with a given hierarchy.
        /// </summary>
        /// <param name="hierarchy">A list of strings which represents the hierarchical order of the package/library</param>
        /// <returns>An object that represents an importable.</returns>
        public obj_ GetLibraryOrPackageObject(List<string> hierarchy)
        {
            obj_ firstImportable = null;
            obj_ currentImportable = null;
            int pos = 0;

            foreach(IImportableInfo info in GetImportableInfos(hierarchy))
            {
                if (info.IsLibrary)
                {
                    obj_ lib;
                    var libInfo = (LibraryInfo)info;

                    if (pos == hierarchy.Count - 1)
                        lib = ConvertToLibraryObject(libInfo);
                    else
                    {
                        var libRecord = new ActivationRecord(libInfo.Name, 0, null);
                        lib = new lib_(libInfo.Name, libRecord);
                    }

                    currentImportable = lib;
                }
                else if (info.IsPackage)
                {
                    var packageInfo = (PackageInfo)info;
                    obj_ package = ConvertToPackageObject(packageInfo);
                    if (currentImportable != null && 
                        currentImportable.GetType() == typeof(lib_))
                    {
                        var recordObj = ((lib_)currentImportable).Val;
                        var record = (ActivationRecord)recordObj;
                        record[packageInfo.Name] = package;
                    }

                    currentImportable = package;
                }

                if (firstImportable == null)
                    firstImportable = currentImportable;
                
                pos++;
            }

            return firstImportable;
        }
    }
}
