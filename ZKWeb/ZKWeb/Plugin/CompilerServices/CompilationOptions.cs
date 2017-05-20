namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Compilation options<br/>
	/// 编译选项<br/>
	/// </summary>
	public class CompilationOptions {
		/// <summary>
		/// Compile with release mode<br/>
		/// It may make some break point not work as excepted, Default is false<br/>
		/// 使用发布模式编译<br/>
		/// 它可能让某些断点不像预期一样工作, 默认值是false<br/>
		/// </summary>
		public bool Release { get; set; }
		/// <summary>
		/// Generate pdb file or not<br/>
		/// Debuggers require pdb file to debug, Default is true<br/>
		/// 是否生成pdb文件<br/>
		/// 调试器调试时需要pdb文件, 默认值是true<br/>
		/// </summary>
		public bool GeneratePdbFile { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public CompilationOptions() {
			Release = false;
			GeneratePdbFile = true;
		}
	}
}
