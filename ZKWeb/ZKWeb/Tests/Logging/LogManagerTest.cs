using System;
using ZKWeb.Logging;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Logging {
	[Tests]
	class LogManagerTest {
		private class TestLogManager : LogManager {
			internal Action<string, string> WhenLog { get; set; }
			public override void Log(string filename, string message) {
				WhenLog(filename, message);
			}
		}

		public void All() {
			using (Application.OverrideIoc()) {
				var logManagerMock = new TestLogManager();
				var lastFileName = "";
				var lastMessage = "";
				logManagerMock.WhenLog = (filename, message) => {
					lastFileName = filename;
					lastMessage = message;
				};
				Application.Ioc.Unregister<LogManager>();
				Application.Ioc.RegisterInstance<LogManager>(logManagerMock);
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
