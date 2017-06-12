using System.Threading;

namespace ZKWeb.Web {
	/// <summary>
	/// Thread pool initializer<br/>
	/// 初始化线程池<br/>
	/// </summary>
	/// <seealso cref="Server.IApplication"/>
	public class ThreadPoolInitializer {
		/// <summary>
		/// Initialize thread pool<br/>
		/// 初始化线程池<br/>
		/// </summary>
		internal protected virtual void Initialize() {
#if NETCORE
			// SetMaxThreads is unsupported in .Net Core
			// But you can use configuration file, see
			// https://github.com/dotnet/cli/issues/889#issuecomment-172975280
			// https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/runtime-configuration-file.md
#else
			// Set max threads as much as possible
			// It's (32767, 32767) on my environment actually
			ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
#endif
		}
	}
}
