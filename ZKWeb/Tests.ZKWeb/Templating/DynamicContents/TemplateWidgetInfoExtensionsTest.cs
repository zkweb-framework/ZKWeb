using ZKWeb.Templating.DynamicContents;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.DynamicContents
{
    [Tests]
    class TemplateWidgetInfoExtensionsTest
    {
        public void GetCacheIsolationPolicyNames()
        {
            var info = new TemplateWidgetInfo();
            info.CacheBy = "Url,Locale";
            var policyNames = info.GetCacheIsolationPolicyNames();
            Assert.Equals(policyNames.Count, 3);
            Assert.Equals(policyNames[0], "Url");
            Assert.Equals(policyNames[1], "Locale");
            Assert.Equals(policyNames[2], "Device");
        }
    }
}
