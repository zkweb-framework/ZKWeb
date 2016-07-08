using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Plugin.AssemblyLoaders;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// roslyn编译服务
	/// </summary>
	internal class RoslynCompilerService : ICompilerService {
		/// <summary>
		/// 编译到的平台名称
		/// </summary>
#if NETCORE
		public string TargetPlatform { get { return "netstandard"; } }
#else
		public string TargetPlatform { get { return "net"; } }
#endif
		/// <summary>
		/// 已载入的命名空间的集合
		/// 用于减少重复载入的时间
		/// </summary>
		protected HashSet<string> LoadedNamespaces { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public RoslynCompilerService() {
			LoadedNamespaces = new HashSet<string>();
		}

		/// <summary>
		/// 找出所有using，并尝试加载里面的程序集
		/// </summary>
		/// <param name="syntaxTrees">语法树列表</param>
		protected void LoadAssembliesFromUsings(IList<SyntaxTree> syntaxTrees) {
			// 从语法树找出所有using
			// 例如"System.Threading"会查找出"System"和"System.Threading"
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var tree in syntaxTrees) {
				foreach (var usingSyntax in ((CompilationUnitSyntax)tree.GetRoot()).Usings) {
					var name = usingSyntax.Name;
					var names = new List<string>();
					while (name != null) {
						// 只有单个名称时是"IdentifierNameSyntax"
						// 有多个名称(X.X)时是"QualifiedNameSyntax"
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
						// 跳过插件的命名空间
						continue;
					}
					names.Reverse();
					for (int c = 1; c <= names.Count; ++c) {
						// 尝试把命名空间当成是程序集载入
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
		/// 编译源代码到程序集
		/// </summary>
		public void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options) {
			// 不指定选项时使用默认选项
			options = options ?? new CompilationOptions();
			// 解析源代码文件
			var syntaxTrees = sourceFiles
				.Select(path => File.ReadAllText(path))
				.Select(text => CSharpSyntaxTree.ParseText(text))
				.ToList();
			// 找出所有using，并尝试加载里面的程序集
			LoadAssembliesFromUsings(syntaxTrees);
			// 引用当前载入的程序集和选项中指定的程序集
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			var references = assemblyLoader.GetLoadedAssemblies()
				.Select(assembly => assembly.Location)
				.Select(path => MetadataReference.CreateFromFile(path))
				.ToList();
			// 设置编译参数
			var optimizationLevel = (options.Debug ?
				OptimizationLevel.Debug : OptimizationLevel.Release);
			var pdbPath = ((!options.GeneratePdbFile) ? null : Path.Combine(
				Path.GetDirectoryName(assemblyPath),
				Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb"));
			// 编译到程序集，出错时抛出例外
			var compilation = CSharpCompilation.Create(assemblyName)
				.WithOptions(new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: optimizationLevel))
				.AddReferences(references)
				.AddSyntaxTrees(syntaxTrees);
			var emitResult = compilation.Emit(assemblyPath, pdbPath);
			if (!emitResult.Success) {
				throw new CompilationException(string.Join("\r\n", emitResult.Diagnostics));
			}
		}
	}
}
