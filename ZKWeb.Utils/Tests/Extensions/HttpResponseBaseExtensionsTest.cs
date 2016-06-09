using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class HttpResponseBaseExtensionsTest {
		public void RedirectByScript() {
			var responseMock = Substitute.For<HttpResponseBase>();
			responseMock.RedirectByScript("testurl");
			responseMock.Received().Clear();
			responseMock.Received().ContentType = "text/html";
			responseMock.Received().Write(Arg.Do<string>(t => t.Contains("testurl")));
			responseMock.Received().End();
		}
	}
}
