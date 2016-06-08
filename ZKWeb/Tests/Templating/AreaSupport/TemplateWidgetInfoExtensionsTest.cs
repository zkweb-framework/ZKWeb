using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.AreaSupport {
	[UnitTest]
	class TemplateWidgetInfoExtensionsTest {
		public void GetCacheIsolationPolicyNames() {
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
