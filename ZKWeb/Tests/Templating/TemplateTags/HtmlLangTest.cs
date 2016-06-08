using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[UnitTest]
	class HtmlLangTest {
		public void Render() {
			LocaleUtils.SetThreadLanguage("zh-CN");
			Assert.Equals(Template.Parse("{% html_lang %}").Render(), "zh-CN");
			LocaleUtils.SetThreadLanguage("en-US");
			Assert.Equals(Template.Parse("{% html_lang %}").Render(), "en-US");
		}
	}
}
