using System.Diagnostics;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// 系统工具类
	/// </summary>
	public static class SystemUtils {
		/// <summary>
		/// 获取当前进程使用的内存，单位是字节
		/// </summary>
		/// <returns></returns>
		public static long GetUsedMemoryBytes() {
			using (var process = Process.GetCurrentProcess()) {
				return process.WorkingSet64;
			}
		}
	}
}
