using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.AreaSupport {
	[UnitTest]
	class TemplateWidgetExtensionsTest {
		public void GetCacheKey() {
			var widget = new TemplateWidget("__test");
			Assert.Equals(widget.GetCacheKey(), "__test");
			widget = new TemplateWidget("__test", new { a = 1 });
			Assert.Equals(widget.GetCacheKey(), "__test{\"a\":1}");
		}
	}
}
