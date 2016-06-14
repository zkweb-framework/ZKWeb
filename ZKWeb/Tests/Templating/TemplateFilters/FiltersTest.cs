using DotLiquid;
using NSubstitute;
using ZKWeb.Localize;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.TemplateFilters {
	[Tests]
	class FiltersTest {
		public void Trans() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				var translateProviderMock = Substitute.For<ITranslateProvider>();
				translateProviderMock.CanTranslate(Arg.Any<string>()).Returns(true);
				translateProviderMock.Translate(Arg.Any<string>())
					.Returns(callInfo => {
						return callInfo.ArgAt<string>(0) == "Original" ? "Translated" : null;
					});
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.RegisterInstance(translateProviderMock);
				Assert.Equals(Template.Parse("{{ 'Original' | trans }}").Render(), "Translated");
				Assert.Equals(Template.Parse("{{ 'NotExist' | trans }}").Render(), "NotExist");
			}
		}

		public void Format() {
			Assert.Equals(Template.Parse(
				"{{ 'name is [0], age is [1]' | format: name, age }}")
					.Render(Hash.FromAnonymousObject(new { name = "John", age = 50 })),
				"name is John, age is 50");
		}

		public void RawHtml() {
			Assert.Equals(
				Template.Parse("{{ html | raw_html }}{{ html }}").Render(
					Hash.FromAnonymousObject(new { html = "<a>'\"test</a>" })),
				"<a>'\"test</a>&lt;a&gt;&#39;&quot;test&lt;/a&gt;");
		}
	}
}
