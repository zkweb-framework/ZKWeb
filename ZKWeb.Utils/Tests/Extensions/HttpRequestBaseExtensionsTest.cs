using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class HttpRequestBaseExtensionsTest {
		public void IsAjaxRequest() {
			using (HttpContextUtils.UseContext("", "POST")) {
				var request = HttpContextUtils.CurrentContext.Request;
				Assert.IsTrue(!request.IsAjaxRequest());
				request.Headers["X-Requested-With"] = "XMLHttpRequest";
				Assert.IsTrue(request.IsAjaxRequest());
			}
		}

		public void Get() {
			using (HttpContextUtils.UseContext("/?a=1&b=2", "POST")) {
				var request = HttpContextUtils.CurrentContext.Request;
				Assert.Equals(request.Get<string>("a"), "1");
				Assert.Equals(request.Get<int>("b"), 2);
				Assert.Equals(request.Get<object>("c"), null);
			}
		}

		public void GetAll() {
			using (HttpContextUtils.UseContext("/?a=1&b=2", "POST")) {
				var request = HttpContextUtils.CurrentContext.Request;
				var allParams = request.GetAll();
				Assert.Equals(allParams.GetOrDefault("a"), "1");
				Assert.Equals(allParams.GetOrDefault("b"), "2");
				Assert.Equals(allParams.GetOrDefault("c"), null);
			}
		}
	}
}
