using DryIoc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Logging;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Logging {
	[UnitTest]
	class LogManagerTest {
		public void All() {
			using (Application.OverrideIoc()) {
				var logManagerMock = new Mock<LogManager>() { CallBase = true };
				var lastFileName = "";
				var lastMessage = "";
				logManagerMock.Setup(l => l.Log(It.IsAny<string>(), It.IsAny<string>()))
					.Callback((string filename, string message) => {
						lastFileName = filename;
						lastMessage = message;
					});
				Application.Ioc.RegisterInstance(logManagerMock.Object);
				var logManager = Application.Ioc.Resolve<LogManager>();
				logManager.LogDebug("Test Debug Message");
				Assert.IsTrueWith(lastFileName.Contains("Debug"), lastFileName);
				Assert.IsTrueWith(lastMessage.Contains("Test Debug Message"), lastMessage);
				logManager.LogInfo("Test Infomation Message");
				Assert.IsTrueWith(lastFileName.Contains("Info"), lastFileName);
				Assert.IsTrueWith(lastMessage.Contains("Test Infomation Message"), lastMessage);
				logManager.LogError("Test Error Message");
				Assert.IsTrueWith(lastFileName.Contains("Error"), lastFileName);
				Assert.IsTrueWith(lastMessage.Contains("Test Error Message"), lastMessage);
				Assert.IsTrueWith(lastMessage.Contains("LogManagerTest.cs"), lastMessage);
				logManager.LogTransaction("Test Transaction Message");
				Assert.IsTrueWith(lastFileName.Contains("Transaction"), lastFileName);
				Assert.IsTrueWith(lastMessage.Contains("Test Transaction Message"), lastMessage);
			}
		}
	}
}
