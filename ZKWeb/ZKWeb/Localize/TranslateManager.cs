using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ZKWeb.Cache;
using ZKWeb.Server;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWeb.Localize {
	/// <summary>
	/// Translate manager
	/// </summary>
	public class TranslateManager : ICacheCleaner {
		/// <summary>
		/// Translated text cache time
		/// Default is 3s, able to override from website configuration
		/// </summary>
		public TimeSpan TranslateCacheTime { get; set; }
		/// <summary>
		/// Translated text cache
		/// { (Language, Orignal text): Translated text, ... }
		/// </summary>
		protected MemoryCache<Pair<string, string>, string> TranslateCache { get; set; }
		/// <summary>
		/// Translate provider cache
		/// { Language: Providers, ... }
		/// </summary>
		protected MemoryCache<string, List<ITranslateProvider>> TranslateProvidersCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public TranslateManager() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			TranslateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TranslateCacheTime, 3));
			TranslateCache = new MemoryCache<Pair<string, string>, string>();
			TranslateProvidersCache = new MemoryCache<string, List<ITranslateProvider>>();
		}

		/// <summary>
		/// Translate text to environment language
		/// </summary>
		/// <param name="text">Original text</param>
		/// <returns></returns>
		public virtual string Translate(string text) {
			var cluture = CultureInfo.CurrentCulture;
			return Translate(text, cluture.Name);
		}

		/// <summary>
		/// Translate text to given language
		/// </summary>
		/// <param name="text">Original text</param>
		/// <param name="code">Language code, eg: zh-CN</param>
		/// <returns></returns>
		public virtual string Translate(string text, string code) {
			// If text is empty, no needs to translate
			if (string.IsNullOrEmpty(text)) {
				return "";
			}
			// Get translated text from cache
			return TranslateCache.GetOrCreate(Pair.Create(code, text), () => {
				// Get translate providers
				var providers = GetTranslateProviders(code);
				// Translate text and store to cache
				// If no provider is able to translate, then return the original text
				foreach (var provider in providers) {
					var translated = provider.Translate(text);
					if (translated != null) {
						return translated;
					}
				}
				return text;
			}, TranslateCacheTime);
		}

		/// <summary>
		/// Get translate providers for given language
		/// </summary>
		/// <param name="code">Language code, eg: zh-CN</param>
		/// <returns></returns>
		public virtual List<ITranslateProvider> GetTranslateProviders(string code) {
			return TranslateProvidersCache.GetOrCreate(code, () => {
				return Application.Ioc.ResolveMany<ITranslateProvider>()
					.Where(p => p.CanTranslate(code))
					.Reverse().ToList();
			}, TranslateCacheTime);
		}

		/// <summary>
		/// Clear cache
		/// </summary>
		public virtual void ClearCache() {
			TranslateCache.Clear();
		}
	}
}
