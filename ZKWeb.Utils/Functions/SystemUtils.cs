using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Functions {
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
