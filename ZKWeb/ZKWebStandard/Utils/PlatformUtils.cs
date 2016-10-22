using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Platform utility functions
	/// </summary>
	public static class PlatformUtils {
		/// <summary>
		/// Running platform is linux or mac
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
		/// Check is running on linux or mac
		/// </summary>
		/// <returns></returns>
		public static bool RunningOnUnix() {
			return IsLinuxOrMac;
		}

		/// <summary>
		/// Check is running on windows
		/// </summary>
		/// <returns></returns>
		public static bool RunningOnWindows() {
			return !IsLinuxOrMac;
		}
	}
}
