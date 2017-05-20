using System.Collections.Generic;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Interface of compiler service<br/>
	/// 编译服务的接口<br/>
	/// </summary>
	public interface ICompilerService {
		/// <summary>
		/// Target platform name, net or netstandard<br/>
		/// 目标平台的名称, net或netstandard<br/>
		/// </summary>
		/// <returns></returns>
		string TargetPlatform { get; }

		/// <summary>
		/// Compile source files to assembly<br/>
		/// 编译源代码到程序集<br/>
		/// </summary>
		/// <param name="sourceFiles">Source file paths</param>
		/// <param name="assemblyName">Assembly name</param>
		/// <param name="assemblyPath">Assembly file path</param>
		/// <param name="options">Compilation options</param>
		void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options = null);
	}
}
