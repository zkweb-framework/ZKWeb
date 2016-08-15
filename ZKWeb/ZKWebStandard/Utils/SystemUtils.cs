using System.Diagnostics;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// System utility functions
	/// </summary>
	public static class SystemUtils {
		/// <summary>
		/// Get used memory in bytes
		/// </summary>
		/// <returns></returns>
		public static long GetUsedMemoryBytes() {
			using (var process = Process.GetCurrentProcess()) {
				return process.WorkingSet64;
			}
		}
	}
}
