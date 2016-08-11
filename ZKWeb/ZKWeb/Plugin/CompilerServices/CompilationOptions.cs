namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Compilation options
	/// </summary>
	public class CompilationOptions {
		/// <summary>
		/// Compile with release mode
		/// It may make some break point not work as excepted, Default is false
		/// </summary>
		public bool Release { get; set; }
		/// <summary>
		/// Generate pdb file or not
		/// Debuggers require pdb file to debug, Default is true
		/// </summary>
		public bool GeneratePdbFile { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public CompilationOptions() {
			Release = false;
			GeneratePdbFile = true;
		}
	}
}
