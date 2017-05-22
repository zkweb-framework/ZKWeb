using System.Diagnostics;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// System utility functions<br/>
	/// <br/>
	/// </summary>
	public static class SystemUtils {
		/// <summary>
		/// Get used memory in bytes<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public static long GetUsedMemoryBytes() {
			using (var process = Process.GetCurrentProcess()) {
				return process.WorkingSet64;
			}
		}
	}
}
