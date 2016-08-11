using System.Collections.Generic;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Interface for compiler service
	/// </summary>
	public interface ICompilerService {
		/// <summary>
		/// Target platform name
		/// </summary>
		/// <returns></returns>
		string TargetPlatform { get; }

		/// <summary>
		/// Compile source files to assembly
		/// </summary>
		/// <param name="sourceFiles">Source file paths</param>
		/// <param name="assemblyName">Assembly name</param>
		/// <param name="assemblyPath">Assembly file path</param>
		/// <param name="options">Compilation options</param>
		void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options = null);
	}
}
