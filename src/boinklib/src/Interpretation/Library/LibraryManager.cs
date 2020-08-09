using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Boink.Interpretation.Library
{
    public interface IImportableInfo
    {
        string Name { get; }

        string Path { get; }

        bool IsPackage { get; }
        bool IsLibrary { get; }
    }

    public struct LibraryInfo : IImportableInfo
    {
        public Dictionary<string, IImportableInfo> Resources { get; }

        public string Name { get; }

        public string Path { get; }

        public bool IsPackage => false;

        public bool IsLibrary => true;

        public LibraryInfo(string libraryName, string libraryPath, Dictionary<string, IImportableInfo> resources)
        {
            Name = libraryName;
            Path = libraryPath;
            Resources = resources;
        }
    }

    public struct PackageInfo : IImportableInfo
    {
        public string Name { get; }

        public string Path { get; }

        public bool IsPackage => true;

        public bool IsLibrary => false;

        public PackageInfo(string packageName, string packagePath)
        {
            Name = packageName;
            Path = packagePath;
        }
    }

    public class DirectoryCache
    {
        Dictionary<string, IImportableInfo> cache;
        
        public DirectoryCache(string rootDir)
        {
            Dictionary<string, IImportableInfo> GetLibraryResources(string dirPath)
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

            cache = GetLibraryResources(rootDir);
        }

        public bool HasLibraryOrPackage(List<string> hierarchy)
        {
            IImportableInfo currentImportable;
            var currentCache = cache;
            foreach(string s in hierarchy)
            {
                if(currentCache.TryGetValue(s, out currentImportable))
                {
                    if(currentImportable.IsLibrary)
                        currentCache = ((LibraryInfo)currentImportable).Resources;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public IImportableInfo GetLibraryOrPackage(List<string> hierarchy)
        {
            IImportableInfo currentImportable = null;
            var currentCache = cache;
            foreach(string s in hierarchy)
            {
                if(currentCache.TryGetValue(s, out currentImportable))
                {
                    if(currentImportable.IsLibrary)
                        currentCache = ((LibraryInfo)currentImportable).Resources;
                }
            }

            return currentImportable;
        }
    }

    public class LibraryManager
    {
        public static HashSet<string> StandardLibraries = new HashSet<string>
        {
            "io",
        };

        public Dictionary<string, Assembly> LoadedLibraries;

        public LibraryManager()
        {
            LoadedLibraries = new Dictionary<string, Assembly>();
        }

        public void LoadLibrary(string dllPath)
        {
            var DLL = Assembly.LoadFile(dllPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dllPath);
            LoadedLibraries[fileNameWithoutExtension] = DLL;
            Console.WriteLine($"Loaded library '{fileNameWithoutExtension}'");
            // InvokeDLLMethod("io", "IO.IO", "writeLine", new object[] {"lol i printed this"});
        }

        public object InvokeDLLMethod(string libName, string className, string methodName, object[] arguments)
        {
            Assembly DLL;
            if (!LoadedLibraries.TryGetValue(libName, out DLL))
                throw new Exception($"Library '{libName}' not found.");

            var classType = DLL.GetType(className);

            if (classType == null)
                throw new Exception($"Class '{className}' not found.");

            MethodInfo methodInfo = classType.GetMethod(methodName);

            if (methodInfo == null)
                throw new Exception($"Method '{methodName}' not found.");

            return methodInfo.Invoke(null, arguments);
        }

    }
}
