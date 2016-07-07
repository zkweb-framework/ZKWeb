#if !NETCORE
using NSubstitute;
using ZKWeb.Logging;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Logging {
	[Tests]
	class LogManagerTest {
		public void All() {
			using (Application.OverrideIoc()) {
				var logManagerMock = Substitute.ForPartsOf<LogManager>();
				var lastFileName = "";
				var lastMessage = "";
				var whenCall = logManagerMock.When(l => l.Log(Arg.Any<string>(), Arg.Any<string>()));
				whenCall.DoNotCallBase();
				whenCall.Do(callInfo => {
					lastFileName = callInfo.ArgAt<string>(0);
					lastMessage = callInfo.ArgAt<string>(1);
				});
				Application.Ioc.Unregister<LogManager>();
				Application.Ioc.RegisterInstance(logManagerMock);
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
#endif
