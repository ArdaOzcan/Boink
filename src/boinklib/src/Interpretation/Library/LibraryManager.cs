using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Class to manage standard libraries, not fully implemented and not used in this version.
    /// </summary>
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
