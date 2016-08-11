using System;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// Exception for compilation error
	/// </summary>
	public class CompilationException : Exception {
		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="message">Error message</param>
		public CompilationException(string message) : base(message) { }
	}
}
