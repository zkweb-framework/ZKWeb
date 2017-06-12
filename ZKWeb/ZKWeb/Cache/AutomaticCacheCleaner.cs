using System;
using System.Threading;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;

namespace ZKWeb.Cache {
	/// <summary>
	/// Automatically clean the cache according to the preset conditions<br/>
	/// 按预置条件自动清理缓存<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// AutomaticCacheCleaner.Start();
	/// </code>
	/// </example>
	public class AutomaticCacheCleaner {
		/// <summary>
		/// Start clean thread<br/>
		/// 启动清理线程<br/>
		/// </summary>
		internal protected virtual void Start() {
			// Read memory usage threshold settings.
			// If no settings present, do not start the cleaner thread.
			// Default check interval is 15s.
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var thresholdMb = configManager.WebsiteConfig.Extra.GetOrDefault<int?>(
				ExtraConfigKeys.ClearCacheAfterUsedMemoryMoreThan);
			var intervalMs = (configManager.WebsiteConfig.Extra.GetOrDefault<int?>(
				ExtraConfigKeys.CleanCacheCheckInterval) ?? 15) * 1000;
			if (thresholdMb == null) {
				return;
			}
			var thread = new Thread(() => {
				while (true) {
					Thread.Sleep(intervalMs);
					try {
						var usedMemory = SystemUtils.GetUsedMemoryBytes() / 1024 / 1024;
						if (usedMemory > thresholdMb.Value) {
							// Clear cache + collect garbage
							var cleaners = Application.Ioc.ResolveMany<ICacheCleaner>();
							cleaners.ForEach(c => c.ClearCache());
							GC.Collect();
						}
					} catch (Exception e) {
						var logManager = Application.Ioc.Resolve<LogManager>();
						logManager.LogError(e.ToString());
					}
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
