using System;
using System.Collections.Generic;
using Boink.Analysis.Semantic;
using Boink.Analysis.Semantic.Symbols;

namespace Boink.Interpretation.Library
{
    /// <summary>
    /// Class to manage standard libraries, not fully implemented and not used in this version.
    /// </summary>
    public class LibraryManager
    {
        public static Dictionary<string, Type> StandardLibraries = new Dictionary<string, Type>
        {
            { "io", typeof(StandardLibrary.io) }
        };

        public Dictionary<Type, SymbolTable> LoadedLibraries;

        public LibraryManager()
        {
            LoadedLibraries = new Dictionary<Type, SymbolTable>();
        }

        public void LoadStandardLibrary(string name)
        {
            var type = StandardLibraries[name];
            var symbolTable = new SymbolTable(name);
            foreach(var method in type.GetMethods())
            {
                var returnType =  method.ReturnType;
                var parameterInfos = method.GetParameters();
                var argTypes = new List<Type>();

                foreach(var info in parameterInfos)
                    argTypes.Add(info.ParameterType);

                symbolTable.Define(new FunctionSymbol(argTypes, method.Name, returnType));      
            }

            LoadedLibraries.Add(type, symbolTable);
        }

        public bool HasStandardLibrary(string name)
        {
            Type type = null;
            StandardLibraries.TryGetValue(name, out type);
            if(type == null)
                return false;
                
            return LoadedLibraries.ContainsKey(type);
        }

        public SymbolTable GetLibrarySymbolTable(string name)
        {
            var type = StandardLibraries[name];
            return LoadedLibraries[type];
        }

        // public void LoadLibrary(string dllPath)
        // {
        //     var DLL = Assembly.LoadFile(dllPath);
        //     var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dllPath);
        //     LoadedLibraries[fileNameWithoutExtension] = DLL;
        //     Console.WriteLine($"Loaded library '{fileNameWithoutExtension}'");
        //     // InvokeDLLMethod("io", "IO.IO", "writeLine", new object[] {"lol i printed this"});
        // }

        // public object InvokeDLLMethod(string libName, string className, string methodName, object[] arguments)
        // {
        //     Assembly DLL;
        //     if (!LoadedLibraries.TryGetValue(libName, out DLL))
        //         throw new Exception($"Library '{libName}' not found.");

        //     var classType = DLL.GetType(className);

        //     if (classType == null)
        //         throw new Exception($"Class '{className}' not found.");

        //     MethodInfo methodInfo = classType.GetMethod(methodName);

        //     if (methodInfo == null)
        //         throw new Exception($"Method '{methodName}' not found.");

        //     return methodInfo.Invoke(null, arguments);
        // }

    }
}
