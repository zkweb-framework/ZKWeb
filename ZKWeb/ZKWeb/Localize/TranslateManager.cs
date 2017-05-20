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
	/// Translate manager<br/>
	/// 翻译管理器<br/>
	/// </summary>
	/// <seealso cref="T"/>
	/// <seealso cref="ITranslateProvider"/>
	/// <example>
	/// <code language="cs">
	/// var translateManager = Application.Ioc.Resolve&lt;TranslateManager&gt;();
	/// var translated = translateManager.Translate("Original");
	/// </code>
	/// </example>
	public class TranslateManager : ICacheCleaner {
		/// <summary>
		/// Translated text cache time<br/>
		/// Default is 3s, able to override from website configuration<br/>
		/// 已翻译文本的缓存时间<br/>
		/// 默认是15秒, 可以使用网站配置覆盖<br/>
		/// </summary>
		public TimeSpan TranslateCacheTime { get; set; }
		/// <summary>
		/// Translated text cache<br/>
		/// { (Language, Orignal text): Translated text, ... }<br/>
		/// 已翻译文本的缓存<br/>
		/// </summary>
		protected IKeyValueCache<Pair<string, string>, string> TranslateCache { get; set; }
		/// <summary>
		/// Translate provider cache<br/>
		/// { Language: Providers, ... }<br/>
		/// 翻译提供器的缓存<br/>
		/// </summary>
		protected IKeyValueCache<string, List<ITranslateProvider>> TranslateProvidersCache { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public TranslateManager() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var cacheFactory = Application.Ioc.Resolve<ICacheFactory>();
			TranslateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TranslateCacheTime, 15));
			TranslateCache = cacheFactory.CreateCache<Pair<string, string>, string>();
			TranslateProvidersCache = cacheFactory.CreateCache<string, List<ITranslateProvider>>();
		}

		/// <summary>
		/// Translate text into the language of the runtime environment<br/>
		/// 翻译文本到运行环境中的语言<br/>
		/// </summary>
		/// <param name="text">Original text</param>
		/// <returns></returns>
		public virtual string Translate(string text) {
			var cluture = CultureInfo.CurrentCulture;
			return Translate(text, cluture.Name);
		}

		/// <summary>
		/// Translate text to the specified language<br/>
		/// 翻译文本到指定的语言<br/>
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
		/// Get translate providers for the given language<br/>
		/// 获取指定语言的翻译提供器列表<br/>
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
		/// Clear cache<br/>
		/// 清理缓存<br/>
		/// </summary>
		public virtual void ClearCache() {
			TranslateCache.Clear();
		}
	}
}
