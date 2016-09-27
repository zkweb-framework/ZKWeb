using System;
using System.Threading;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;

namespace ZKWeb.Cache {
	/// <summary>
	/// Automatic cache cleaner
	/// </summary>
	internal static class AutomaticCacheCleaner {
		/// <summary>
		/// Start cleaner
		/// </summary>
		internal static void Start() {
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
