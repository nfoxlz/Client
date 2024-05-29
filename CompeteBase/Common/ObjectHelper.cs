using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Compete.Common
{
    public sealed class ObjectHelper
    {
        private const short maxRecursion = 20;

        //private static readonly ICollection<ObjectHelper> helpers = new HashSet<ObjectHelper>();
        private static readonly ICollection<ObjectHelper> helpers = [];

        public static ObjectHelper Default { get; private set; } = new();

        public static IDictionary<string, Assembly> AssemblyDictionary { get; } = new Dictionary<string, Assembly>();

        static ObjectHelper() => AddAssembly(typeof(ObjectHelper));

        public static void AddAssembly<T>() => AddAssembly(typeof(T));

        public static void AddAssembly(Type type)
        {
            var name = string.Format("{0}.dll", type.Assembly.GetName().Name!);
            if (!AssemblyDictionary.ContainsKey(name))
                AssemblyDictionary.Add(name, type.Assembly);
        }

        private AssemblyLoadContext loadContext = new(null, true);

        //private readonly ICollection<string> paths = new HashSet<string>();
        private readonly ICollection<string> paths = [];

        private short recursiveCount = 0;

        public static void ReloadAll()
        {
            foreach (var helper in helpers)
                helper.loadContext.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            foreach (var helper in helpers)
            {
                helper.loadContext = new AssemblyLoadContext(null, true);
                foreach (var path in helper.paths)
                    helper.LoadAssembly(path);
            }
        }

        public ObjectHelper() => helpers.Add(this);

        ~ObjectHelper() => helpers.Remove(this);

        public string LibraryPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        private void Free()
        {
            loadContext.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            loadContext = new AssemblyLoadContext(null, true);
        }

        public void Clear()
        {
            lock (paths)
            {
                Free();
                paths.Clear();
            }
        }

        public void Reload()
        {
            Free();
            foreach(var path in paths)
                LoadAssembly(path);
        }

        public static string GetAssemblyName(Assembly assembly)
        {
            if (assembly.FullName!.StartsWith("system.", StringComparison.CurrentCultureIgnoreCase))
                return assembly.FullName;

            if (!string.IsNullOrWhiteSpace(assembly.Location) && Default.IsLoad(assembly.FullName))
                return Path.GetFileName(assembly.Location);

            return assembly.FullName;
        }

        public Assembly? LoadAssembly(string path)
        {
            if (AssemblyDictionary.TryGetValue(path, out Assembly? value))
                return value;

            var assemblyPath = path;
            if (!File.Exists(assemblyPath))
            {
                assemblyPath = Path.Combine(LibraryPath, assemblyPath);
                if (!File.Exists(assemblyPath))
                    try
                    {
                        return Assembly.Load(path);
                    }
                    catch (FileNotFoundException)
                    {
                        return null;
                    }
            }

            Assembly? result = null;
            using (Stream stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
                try
                {
                    return loadContext.LoadFromStream(stream);
                }
                catch (FileLoadException)
                {
                    if (recursiveCount >= maxRecursion)
                        return null;
                    recursiveCount++;
                    Reload();
                    result = LoadAssembly(path);
                    recursiveCount--;
                }
                finally
                {
                    stream.Close();
                }

            if (result != null)
                lock (paths)
                    if (!paths.Contains(path))
                        paths.Add(path);

            return result;
        }

        //public object? CreateInstance(string typeName)
        //{

        //}

        public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type) => type == typeof(DBNull) ? DBNull.Value : Activator.CreateInstance(type);

        //[RequiresUnreferencedCode("ObjectHelper.CreateInstance is not supported with trimming. Use Type.GetType instead.")]
        public static object? CreateInstance(Assembly assembly, string typeName) => typeName == typeof(DBNull).FullName ? DBNull.Value : assembly.CreateInstance(typeName);

        public object? CreateInstance(TypeSetting setting) => CreateInstance(setting.Assembly, setting.Type);

        public object? CreateInstance(string path, string typeName)
        {
            var assembly = LoadAssembly(path);
            return null == assembly ? null : CreateInstance(assembly, typeName);
        }

        public Type? GetType(string path, string typeName) => LoadAssembly(path)?.GetType(typeName);

        public bool IsLoad(string name) => (from assembly in loadContext.Assemblies
                                            where assembly.FullName == name
                                            select assembly).Any();
    }
}
