using DotLiquid;
using ZKWeb.Templating;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[Tests]
	class RawHtmlTest {
		public void Render() {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			Assert.Equals(
				Template.Parse("{% raw_html html %}{{ html }}").Render(
					templateManager.CreateHash(new { html = "<a>'\"test</a>" })),
				"<a>'\"test</a>&lt;a&gt;&#39;&quot;test&lt;/a&gt;");
			Assert.Equals(Template.Parse("{% raw_html \"abc\" %}").Render(), "abc");
		}
	}
}
