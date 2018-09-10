using Newtonsoft.Json;
using ZKWeb.Localize;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Localize {
	[Tests]
	class TJsonConverterTest {
		public void SerializeT() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TranslateManagerTest.TestTranslateProviderCN>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				LocaleUtils.SetThreadLanguage("zh-CN");
				var obj = new { field = new T("Original") };
				var json = JsonConvert.SerializeObject(obj);
				Assert.Equals(json, "{\"field\":\"TranslatedCN\"}");
			}
		}

		public void DeserializeT() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TranslateManagerTest.TestTranslateProviderCN>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				LocaleUtils.SetThreadLanguage("zh-CN");
				var json = "\"Original\"";
				var t = JsonConvert.DeserializeObject<T>(json);
				Assert.Equals(t.ToString(), "TranslatedCN");
			}
		}
	}
}
