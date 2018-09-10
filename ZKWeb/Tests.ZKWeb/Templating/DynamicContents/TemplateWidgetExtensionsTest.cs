using ZKWeb.Templating.DynamicContents;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.DynamicContents {
	[Tests]
	class TemplateWidgetExtensionsTest {
		public void GetCacheKey() {
			var widget = new TemplateWidget("__test");
			Assert.Equals(widget.GetCacheKey(), "__test");
			widget = new TemplateWidget("__test", new { a = 1 });
			Assert.Equals(widget.GetCacheKey(), "__test{\"a\":1}");
		}
	}
}
