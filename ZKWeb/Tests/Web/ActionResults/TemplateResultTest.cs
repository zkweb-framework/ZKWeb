using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Templating;
using ZKWeb.Tests.Server;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class TemplateResultTest {
		public void WriteResponse() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateManager>();
				Application.Ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
				layout.WritePluginFile("PluginA", "templates/__test_a.html", "test a {{ name }}");
				layout.WritePluginFile("PluginB", "templates/__test_b.html", "test b {{ name }}");

				var result = new TemplateResult("__test_a.html", new { name = "asd" });
				var responseMock = new HttpResponseMock();
				responseMock.outputStream = new MemoryStream();
				result.WriteResponse(responseMock);
				responseMock.outputStream.Seek(0, SeekOrigin.Begin);
				Assert.Equals(new StreamReader(responseMock.outputStream).ReadToEnd(), "test a asd");

				result = new TemplateResult("__test_b.html", new { name = "asd" });
				responseMock = new HttpResponseMock();
				responseMock.outputStream = new MemoryStream();
				result.WriteResponse(responseMock);
				responseMock.outputStream.Seek(0, SeekOrigin.Begin);
				Assert.Equals(new StreamReader(responseMock.outputStream).ReadToEnd(), "test b asd");
			}
		}
	}
}
