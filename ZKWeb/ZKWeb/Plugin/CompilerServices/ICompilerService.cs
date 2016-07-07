using System.Collections.Generic;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// 编译服务的接口
	/// </summary>
	public interface ICompilerService {
		/// <summary>
		/// 获取编译到的平台名称
		/// </summary>
		/// <returns></returns>
		string TargetPlatform { get; }

		/// <summary>
		/// 编译源代码到程序集
		/// </summary>
		/// <param name="sourceFiles">源代码文件列表</param>
		/// <param name="assemblyName">保存的程序集名称</param>
		/// <param name="assemblyPath">保存的程序集路径</param>
		/// <param name="options">编译选项</param>
		void Compile(IList<string> sourceFiles,
			string assemblyName, string assemblyPath, CompilationOptions options = null);
	}
}
