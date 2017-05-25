using System.Diagnostics;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// System utility functions<br/>
	/// 系统的工具函数<br/>
	/// </summary>
	public static class SystemUtils {
		/// <summary>
		/// Get used memory in bytes<br/>
		/// 获取占用的内存大小<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var value = SystemUtils.GetUsedMemoryBytes();
		/// </code>
		/// </example>
		public static long GetUsedMemoryBytes() {
			using (var process = Process.GetCurrentProcess()) {
				return process.WorkingSet64;
			}
		}
	}
}
