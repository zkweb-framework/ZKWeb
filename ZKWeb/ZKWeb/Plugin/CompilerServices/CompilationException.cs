using System;

namespace ZKWeb.Plugin.CompilerServices {
	/// <summary>
	/// 编译错误
	/// </summary>
	public class CompilationException : Exception {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="message">错误信息</param>
		public CompilationException(string message) : base(message) { }
	}
}
