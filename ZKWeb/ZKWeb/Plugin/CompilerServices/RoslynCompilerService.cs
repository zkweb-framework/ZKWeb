using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Plugin.AssemblyLoaders;

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
		/// 编译源代码到程序集
		/// </summary>
		public void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options) {
			// 不指定选项时使用默认选项
			options = options ?? new CompilationOptions();
#if !NETCORE
			// 确保部分有可能延迟加载的程序集已载入
			typeof(NHibernate.IQuery).ToString();
			typeof(FluentNHibernate.IMappingProvider).ToString();
			typeof(DotLiquid.ILiquidizable).ToString();
			typeof(NSubstitute.Substitute).ToString();
#endif
			// 引用当前引用的程序集，和选项中指定的程序集
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			var references = assemblyLoader.GetLoadedAssemblies()
				.Select(assembly => assembly.Location)
				.Concat(options.ReferenceAssemblyPaths)
				.Select(path => MetadataReference.CreateFromFile(path));
			// 解析源代码文件
			var syntaxTrees = sourceFiles
				.Select(path => File.ReadAllText(path))
				.Select(text => CSharpSyntaxTree.ParseText(text));
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