using System;
using System.Collections.Generic;
using System.Reflection;

using Boink.Analysis.Semantic;
using Boink.Analysis.Semantic.Symbols;

using Boink.Types;

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

        public static Dictionary<Type, List<MethodInfo>> MethodsCache = new Dictionary<Type, List<MethodInfo>>();

        public Dictionary<Type, SymbolTable> LoadedLibraries;

        public LibraryManager()
        {
            LoadedLibraries = new Dictionary<Type, SymbolTable>();
        }

        public void LoadStandardLibrary(string name)
        {
            var type = StandardLibraries[name];
            var symbolTable = new SymbolTable(name);

            List<MethodInfo> realInfos = new List<MethodInfo>();
            foreach (var method in type.GetMethods())
            {
                if(method.DeclaringType == type)
                    realInfos.Add(method);
            }
            MethodsCache[type] = realInfos;

            foreach(var method in MethodsCache[type])
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

        public static stdfunc_ GetStandardFunctionObject(MethodInfo methodInfo) => new stdfunc_(methodInfo.Name, methodInfo);

        public static lib_ GetStandardLibraryObject(string name)
        {
            var type = StandardLibraries[name];
            var libRecord = new ActivationRecord(name, 0, null);
            foreach(var method in MethodsCache[type])
            {
                var returnType =  method.ReturnType;
                var parameterInfos = method.GetParameters();
                var argTypes = new List<Type>();

                foreach(var info in parameterInfos)
                    argTypes.Add(info.ParameterType);

                libRecord.DefineVar(method.Name, GetStandardFunctionObject(method));     
            }

            return new lib_(name, libRecord);
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
