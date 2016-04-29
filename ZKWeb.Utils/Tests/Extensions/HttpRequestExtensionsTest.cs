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
	class HttpRequestExtensionsTest {
		public void IsAjaxRequest() {
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(r => r.Headers)
				.Returns(HttpUtility.ParseQueryString("X-Requested-With=XMLHttpRequest"));
			var request = requestMock.Object;
			Assert.IsTrue(request.IsAjaxRequest());
		}
	}
}
