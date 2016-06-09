using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;
using ZKWeb.Tests.Server;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class FileResultTest {
		public void WriteResponse() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WriteAppDataFile("static/__test.txt", "test contents");
				var pathManager = Application.Ioc.Resolve<PathManager>();
				var resourcePath = pathManager.GetResourceFullPath("static/__test.txt");
				var lastModified = File.GetLastWriteTimeUtc(resourcePath).Truncate();
				Assert.Equals(File.ReadAllText(resourcePath), "test contents");

				var result = new FileResult(resourcePath);
				var responseMock = Substitute.For<HttpResponseBase>();
				var cacheMock = Substitute.For<HttpCachePolicyBase>();
				responseMock.Cache.Returns(cacheMock);
				result.WriteResponse(responseMock);
				responseMock.Received().WriteFile(resourcePath);
				cacheMock.Received().SetLastModified(lastModified);

				result = new FileResult(resourcePath, DateTime.UtcNow.AddDays(1));
				responseMock.ClearReceivedCalls();
				cacheMock.ClearReceivedCalls();
				result.WriteResponse(responseMock);
				responseMock.Received().WriteFile(resourcePath);
				cacheMock.Received().SetLastModified(lastModified);

				result = new FileResult(resourcePath, lastModified);
				responseMock.ClearReceivedCalls();
				cacheMock.ClearReceivedCalls();
				result.WriteResponse(responseMock);
				responseMock.DidNotReceive().WriteFile(Arg.Any<string>());
				responseMock.Received().StatusCode = 304;
				responseMock.Received().SuppressContent = true;
				cacheMock.Received().SetLastModified(lastModified);
			}
		}
	}
}
