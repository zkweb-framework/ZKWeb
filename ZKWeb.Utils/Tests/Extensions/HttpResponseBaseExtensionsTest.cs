using Moq;
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
			var responseMock = new Mock<HttpResponseBase>();
			responseMock.Setup(r => r.Clear()).Callback(() => { }).Verifiable();
			responseMock.SetupSet(r => r.ContentType = It.Is<string>(t => t == "text/html")).Verifiable();
			responseMock.Setup(r => r.Write(It.Is<string>(t => t.Contains("testurl")))).Verifiable();
			responseMock.Setup(r => r.End()).Callback(() => { }).Verifiable();
			responseMock.Object.RedirectByScript("testurl");
			responseMock.VerifyAll();
		}
	}
}
