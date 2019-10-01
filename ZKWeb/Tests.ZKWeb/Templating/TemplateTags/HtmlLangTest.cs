using DotLiquid;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.TemplateTags
{
    [Tests]
    class HtmlLangTest
    {
        public void Render()
        {
            LocaleUtils.SetThreadLanguage("zh-CN");
            Assert.Equals(Template.Parse("{% html_lang %}").Render(), "zh-CN");
            LocaleUtils.SetThreadLanguage("en-US");
            Assert.Equals(Template.Parse("{% html_lang %}").Render(), "en-US");
        }
    }
}
