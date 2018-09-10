using DotLiquid;
using ZKWeb.Localize;
using ZKWeb.Templating;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.TemplateFilters {
	[Tests]
	class FiltersTest {
		private class TestTranslateProvider : ITranslateProvider {
			public bool CanTranslate(string code) {
				return true;
			}

			public string Translate(string text) {
				return text == "Original" ? "Translated" : null;
			}
		}

		public void Trans() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				var translateProviderMock = new TestTranslateProvider();
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.RegisterInstance<ITranslateProvider>(translateProviderMock);
				Assert.Equals(Template.Parse("{{ 'Original' | trans }}").Render(), "Translated");
				Assert.Equals(Template.Parse("{{ 'NotExist' | trans }}").Render(), "NotExist");
			}
		}

		public void Format() {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			Assert.Equals(Template.Parse(
				"{{ 'name is [0], age is [1]' | format: name, age }}")
					.Render(templateManager.CreateHash(new { name = "John", age = 50 })),
				"name is John, age is 50");
		}

		public void RawHtml() {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			Assert.Equals(
				Template.Parse("{{ html | raw_html }}{{ html }}").Render(
					templateManager.CreateHash(new { html = "<a>'\"test</a>" })),
				"<a>'\"test</a>&lt;a&gt;&#39;&quot;test&lt;/a&gt;");
		}
	}
}
