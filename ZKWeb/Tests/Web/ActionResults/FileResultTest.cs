using Moq;
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
				var responseMock = new Mock<HttpResponseBase>();
				var cacheMock = new Mock<HttpCachePolicyBase>();
				cacheMock.Setup(
					c => c.SetLastModified(It.Is<DateTime>(d => d == lastModified))).Verifiable();
				responseMock.Setup(r => r.Cache).Returns(cacheMock.Object);
				responseMock.Setup(r => r.WriteFile(It.Is<string>(s => s == resourcePath))).Verifiable();
				result.WriteResponse(responseMock.Object);
				responseMock.Verify();
				cacheMock.Verify();

				result = new FileResult(resourcePath, DateTime.UtcNow.AddDays(1));
				cacheMock.ResetCalls();
				responseMock = new Mock<HttpResponseBase>();
				responseMock.Setup(r => r.Cache).Returns(cacheMock.Object);
				responseMock.Setup(r => r.WriteFile(It.Is<string>(s => s == resourcePath))).Verifiable();
				result.WriteResponse(responseMock.Object);
				responseMock.Verify();
				cacheMock.Verify();

				result = new FileResult(resourcePath, lastModified);
				cacheMock.ResetCalls();
				responseMock = new Mock<HttpResponseBase>();
				responseMock.Setup(r => r.Cache).Returns(cacheMock.Object);
				responseMock.SetupSet(r => r.StatusCode = 304);
				responseMock.SetupSet(r => r.SuppressContent = true);
				result.WriteResponse(responseMock.Object);
				responseMock.Verify();
				cacheMock.Verify();
			}
		}
	}
}
