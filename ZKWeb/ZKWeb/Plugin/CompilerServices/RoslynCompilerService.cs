using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Plugin.AssemblyLoaders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using ZKWebStandard.Utils;
using System.FastReflection;
using System.Reflection;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Roslyn compiler service<br/>
	/// 基于Roslyn的编译服务<br/>
	/// </summary>
	internal class RoslynCompilerService : ICompilerService {
		/// <summary>
		/// Target platform name, net or netstandard<br/>
		/// 目标平台的名称, net或netstandard<br/>
		/// </summary>
#if NETCORE
		public string TargetPlatform { get { return "netstandard"; } }
#else
		public string TargetPlatform { get { return "net"; } }
#endif
		/// <summary>
		/// Loaded namespaces, for reducing load time<br/>
		/// 已加载的命名空间, 用于减少加载时间<br/>
		/// </summary>
		protected HashSet<string> LoadedNamespaces { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public RoslynCompilerService() {
			LoadedNamespaces = new HashSet<string>();
		}

		/// <summary>
		/// Find all using directive<br/>
		/// And try to load the namespace as assembly<br/>
		/// 寻找源代码中的所有using指令<br/>
		/// 并尝试加载命名空间对应的程序集<br/>
		/// </summary>
		/// <param name="syntaxTrees">Syntax trees</param>
		protected void LoadAssembliesFromUsings(IList<SyntaxTree> syntaxTrees) {
			// Find all using directive
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var tree in syntaxTrees) {
				foreach (var usingSyntax in ((CompilationUnitSyntax)tree.GetRoot()).Usings) {
					var name = usingSyntax.Name;
					var names = new List<string>();
					while (name != null) {
						// The type is "IdentifierNameSyntax" if it's single identifier
						// eg: System
						// The type is "QualifiedNameSyntax" if it's contains more than one identifier
						// eg: System.Threading
						if (name is QualifiedNameSyntax) {
							var qualifiedName = (QualifiedNameSyntax)name;
							var identifierName = (IdentifierNameSyntax)qualifiedName.Right;
							names.Add(identifierName.Identifier.Text);
							name = qualifiedName.Left;
						} else if (name is IdentifierNameSyntax) {
							var identifierName = (IdentifierNameSyntax)name;
							names.Add(identifierName.Identifier.Text);
							name = null;
						}
					}
					if (names.Contains("src")) {
						// Ignore if it looks like a namespace from plugin 
						continue;
					}
					names.Reverse();
					for (int c = 1; c <= names.Count; ++c) {
						// Try to load the namespace as assembly
						// eg: will try "System" and "System.Threading" from "System.Threading"
						var usingName = string.Join(".", names.Take(c));
						if (LoadedNamespaces.Contains(usingName)) {
							continue;
						}
						try {
							assemblyLoader.Load(usingName);
						} catch {
						}
						LoadedNamespaces.Add(usingName);
					}
				}
			}
		}

		/// <summary>
		/// Compile source files to assembly<br/>
		/// 编译源代码到程序集<br/>
		/// </summary>
		public void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options) {
			// Use default options if `options` is null
			options = options ?? new CompilationOptions();
			// Parse source files into syntax trees
			// Also define NETCORE for .Net Core
			var parseOptions = CSharpParseOptions.Default;
#if NETCORE
			parseOptions = parseOptions.WithPreprocessorSymbols("NETCORE");
#endif
			var syntaxTrees = sourceFiles
				.Select(path => CSharpSyntaxTree.ParseText(
					File.ReadAllText(path), parseOptions, path, Encoding.UTF8))
				.ToList();
			// Find all using directive and load the namespace as assembly
			// It's for resolve assembly dependencies of plugin
			LoadAssembliesFromUsings(syntaxTrees);
			// Add loaded assemblies to compile references
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			var references = assemblyLoader.GetLoadedAssemblies()
				.Select(assembly => assembly.Location)
				.Select(path => MetadataReference.CreateFromFile(path))
				.ToList();
			// Set roslyn compilation options
			// Generate pdb file only supported on windows,
			// because Microsoft.DiaSymReader.Native only have windows runtimes
			if (!PlatformUtils.RunningOnWindows()) {
				options.GeneratePdbFile = false;
			}
			var optimizationLevel = (options.Release ?
				OptimizationLevel.Release : OptimizationLevel.Debug);
			var pdbPath = ((!options.GeneratePdbFile) ? null : Path.Combine(
				Path.GetDirectoryName(assemblyPath),
				Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb"));
			// Create compilation options and set IgnoreCorLibraryDuplicatedTypes flag
			// To avoid error like The type 'Path' exists in both
			// 'System.Runtime.Extensions, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
			// and
			// 'System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
			var compilationOptions = new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				optimizationLevel: optimizationLevel);
			var withTopLevelBinderFlagsMethod = compilationOptions.GetType()
				.FastGetMethod("WithTopLevelBinderFlags", BindingFlags.Instance | BindingFlags.NonPublic);
			var binderFlagsType = withTopLevelBinderFlagsMethod.GetParameters()[0].ParameterType;
			compilationOptions = (CSharpCompilationOptions)withTopLevelBinderFlagsMethod.FastInvoke(
				compilationOptions,
				binderFlagsType.GetField("IgnoreCorLibraryDuplicatedTypes").GetValue(binderFlagsType));
			// Compile to assembly, throw exception if error occurred
			var compilation = CSharpCompilation.Create(assemblyName)
				.WithOptions(compilationOptions)
				.AddReferences(references)
				.AddSyntaxTrees(syntaxTrees);
			var emitResult = compilation.Emit(assemblyPath, pdbPath);
			if (!emitResult.Success) {
				throw new CompilationException(string.Join("\r\n",
					emitResult.Diagnostics.Where(d => d.WarningLevel == 0)));
			}
		}
	}
}
