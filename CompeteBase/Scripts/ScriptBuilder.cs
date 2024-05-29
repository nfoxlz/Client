// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/1/26 周六 18:19:10 LeeZheng 新建。
// ======================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Compete.Scripts
{
    /// <summary>
    /// ScriptBuilder 类。
    /// </summary>
    public sealed class ScriptBuilder
    {
        public const string DefaultLanguage = "CSharp";

        private static string gcPath = Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location)!;

        private static readonly IEnumerable<MetadataReference>? references = new[]
        {
            MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(typeof(DynamicAttribute).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(DataRow).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MarshalByValueComponent).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FrameworkElement).GetTypeInfo().Assembly.Location),

            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.Linq.dll")),
            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.ComponentModel.Primitives.dll")),
            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.ComponentModel.dll")),
            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.Xml.ReaderWriter.dll")),
            MetadataReference.CreateFromFile(Path.Combine(gcPath, "System.Private.Xml.dll")),
        };

        private static readonly CSharpCompilationOptions cSharpCompilationOptions = new(OutputKind.DynamicallyLinkedLibrary);

        private static readonly VisualBasicCompilationOptions visualBasicCompilationOptions = new(OutputKind.DynamicallyLinkedLibrary);

        public string Language { get; set; } = "CSharp";

        //public IEnumerable<string>? ReferencedAssemblies { get; set; }

        public string Sources { get; set; } = string.Empty;

        public ICollection<string>? Errors { get; private set; }

        //public bool GenerateExecutable { get; set; } = false;

        //public bool GenerateInMemory { get; set; } = true;

        private Compilation CreateCompilation(string? assemblyName) => Language.ToUpper() switch
        {
            "CSHARP" or "CS" or "C#" => CSharpCompilation.Create(assemblyName, [CSharpSyntaxTree.ParseText(Sources)], references, cSharpCompilationOptions),
            "VISUALBASIC" or "VB" => VisualBasicCompilation.Create(assemblyName, [VisualBasicSyntaxTree.ParseText(Sources)], references, visualBasicCompilationOptions),
            _ => throw new PlatformNotSupportedException(),
        };

        public Assembly? Build() => Build(Path.GetTempFileName(), true);

        public Assembly? Build(string path, bool isDelete = false)
        {
            //CompilerParameters compilerParameters = new()
            //{
            //    GenerateExecutable = GenerateExecutable,
            //    GenerateInMemory = GenerateInMemory,
            //    TempFiles = new TempFileCollection(Path.GetTempPath()),

            //};
            //foreach (var assembly in ReferencedAssemblies!)
            //    compilerParameters.ReferencedAssemblies.Add(assembly);

            //var provider = CodeDomProvider.CreateProvider(Language);
            //CompilerResults results = provider.CompileAssemblyFromSource(compilerParameters, Sources);

            //var errors = new List<string>();
            //if (results.Errors.HasErrors)
            //{
            //    foreach (CompilerError err in results.Errors)
            //        errors.Add(err.ErrorText);
            //    Errors = errors;
            //    return null;
            //}

            //return results.CompiledAssembly;

            var result = CreateCompilation(Path.GetFileName(path)).Emit(path);

            if (result.Success)
                try
                {
                    return Assembly.Load(File.ReadAllBytes(path));
                }
                finally
                {
                    Task.Run(() =>
                    {
                        if (isDelete)
                            File.Delete(path);
                    });
                }
            else
            {
                var errors = new List<string>();
                foreach (Diagnostic diagnostic in result.Diagnostics)
                    errors.Add(diagnostic.GetMessage());
                Errors = errors;
                return null;
            }
        }

        public static string GetAssemblyFileName(string path, string className) => string.Format("{0}.dll", Path.Combine(path, className));

        public static Type? GetType(string path, string template, string code, string className, string language = "CSharp")
        {
            var bilder = new ScriptBuilder
            {
                Language = language,
                //ReferencedAssemblies = ScriptTemplates.DataReferencedAssemblies,
                Sources = string.Format(template, GetName(className), code),
            };

            var assemblyPath = GetAssemblyFileName(path, className);
            var assembly = bilder.Build(assemblyPath);
            //Debug.Assert(null != assembly && null == bilder.Errors, $"脚本编译出差。\r\n{string.Join("\r\n", bilder.Errors?.ToString() ?? string.Empty)}");
            if (null == assembly || null != bilder.Errors && 0 < bilder.Errors.Count)
            {
                var info = new FileInfo(assemblyPath);
                if (info.Length == 0)
                    info.Delete();
                return null;
            }

            return assembly.GetType(className);
        }

        private static string GetName(string name)
        {
            var index = name.LastIndexOf('.') + 1;
            return index > 0 && index < name.Length ? name[index..] : name;
        }

        public static Type? GetType(string template, string code, string className, string language = "CSharp")
        {
            var bilder = new ScriptBuilder
            {
                Language = language,
                //ReferencedAssemblies = ScriptTemplates.DataReferencedAssemblies,
                Sources = string.Format(template, GetName(className), code),
            };
            
            var assembly = bilder.Build();
            Debug.Assert(null != assembly && null == bilder.Errors, $"脚本编译出差。\r\n{string.Join("\r\n", bilder.Errors?.ToString() ?? string.Empty)}");

            return assembly.GetType(className);
        }

        public static MethodInfo? GetMethod(string template, string code, string className, string methodName, string language = "CSharp") => GetType(template, code, className, language)?.GetMethod(methodName);
    }
}
