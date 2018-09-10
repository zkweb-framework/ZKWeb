using ZKWeb.Localize;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Localize {
	[Tests]
	class TranslateManagerTest {
		public void Translate() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TestTranslateProviderCN>();
				Application.Ioc.RegisterMany<TestTranslateProviderUS>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				var translateManager = Application.Ioc.Resolve<TranslateManager>();
				Assert.Equals(translateManager.Translate("Original", "zh-CN"), "TranslatedCN");
				Assert.Equals(translateManager.Translate("Original", "en-US"), "TranslatedUS");
				LocaleUtils.SetThreadLanguage("zh-CN");
				Assert.Equals(translateManager.Translate("Original"), "TranslatedCN");
				LocaleUtils.SetThreadLanguage("en-US");
				Assert.Equals(translateManager.Translate("Original"), "TranslatedUS");
			}
		}

		public void GetTranslateProviders() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TestTranslateProviderCN>();
				Application.Ioc.RegisterMany<TestTranslateProviderUS>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				var translateManager = Application.Ioc.Resolve<TranslateManager>();
				var providersCN = translateManager.GetTranslateProviders("zh-CN");
				var providersUS = translateManager.GetTranslateProviders("en-US");
				Assert.Equals(providersCN.Count, 1);
				Assert.Equals(providersCN[0].GetType(), typeof(TestTranslateProviderCN));
				Assert.Equals(providersUS.Count, 1);
				Assert.Equals(providersUS[0].GetType(), typeof(TestTranslateProviderUS));
			}
		}

		internal class TestTranslateProviderCN : ITranslateProvider {
			public bool CanTranslate(string code) {
				return code == "zh-CN";
			}

			public string Translate(string text) {
				return text == "Original" ? "TranslatedCN" : null;
			}
		}

		internal class TestTranslateProviderUS : ITranslateProvider {
			public bool CanTranslate(string code) {
				return code == "en-US";
			}

			public string Translate(string text) {
				return text == "Original" ? "TranslatedUS" : null;
			}
		}
	}
}
