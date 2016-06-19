using System;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWebStandard.Tests.Web {
	[Tests]
	class HttpManagerTest {
		public void OverrideContext() {
			Assert.Equals(HttpManager.CurrentContext.Request.Path, "/");
			using (HttpManager.OverrideContext("a", "GET")) {
				using (HttpManager.OverrideContext("b", "GET")) {
					Assert.Equals(HttpManager.CurrentContext.Request.Path, "/b");
				}
				Assert.Equals(HttpManager.CurrentContext.Request.Path, "/a");
			}
			Assert.Equals(HttpManager.CurrentContext.Request.Path, "/");
		}
	}
}
