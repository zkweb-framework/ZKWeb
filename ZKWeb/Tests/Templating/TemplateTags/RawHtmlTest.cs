using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[UnitTest]
	class RawHtmlTest {
		public void Render() {
			Assert.Equals(
				Template.Parse("{% raw_html html %}{{ html }}").Render(
					Hash.FromAnonymousObject(new { html = "<a>'\"test</a>" })),
				"<a>'\"test</a>&lt;a&gt;&#39;&quot;test&lt;/a&gt;");
			Assert.Equals(Template.Parse("{% raw_html \"abc\" %}").Render(), "abc");
		}
	}
}
