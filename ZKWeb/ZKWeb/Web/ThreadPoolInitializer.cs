using System.Threading;

namespace ZKWeb.Web {
	/// <summary>
	/// 设置线程池的参数
	/// </summary>
	internal static class ThreadPoolInitializer {
		/// <summary>
		/// 设置线程池的参数
		/// </summary>
		internal static void Initialize() {
#if NETCORE
			// .Net Core不支持在代码中设置线程池使用的数量
			// 但可以在配置文件或环境变量中指定，请参考
			// https://github.com/dotnet/cli/issues/889#issuecomment-172975280
			// https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/runtime-configuration-file.md
#else
			// 设置线程池使用尽可能多的线程
			// 实际本机设置的数量是(32767, 32767)
			ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
#endif
		}
	}
}
