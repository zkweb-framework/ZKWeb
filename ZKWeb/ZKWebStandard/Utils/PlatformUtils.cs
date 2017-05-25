using System;
using System.Runtime.InteropServices;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Platform utility functions<br/>
	/// 平台的工具函数<br/>
	/// </summary>
	public static class PlatformUtils {
		/// <summary>
		/// Running platform is linux or mac<br/>
		/// 运行的系统是否Linux或Mac<br/>
		/// </summary>
		private static bool IsLinuxOrMac = false;

		static PlatformUtils() {
#if NETCORE
			IsLinuxOrMac = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
				RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
			int platform = (int)Environment.OSVersion.Platform;
			IsLinuxOrMac = ((platform == 4) || (platform == 6) || (platform == 128));
#endif
		}

		/// <summary>
		/// Check is running on linux or mac<br/>
		/// 判断运行的系统是否Linux或Mac<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var isOnUnix = PlatformUtils.RunningOnUnix();
		/// </code>
		/// </example>
		public static bool RunningOnUnix() {
			return IsLinuxOrMac;
		}

		/// <summary>
		/// Check is running on windows<br/>
		/// 判断运行的系统是否Windows<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var isOnWindows = PlatformUtils.RunningOnWindows();
		/// </code>
		/// </example>
		public static bool RunningOnWindows() {
			return !IsLinuxOrMac;
		}
	}
}
