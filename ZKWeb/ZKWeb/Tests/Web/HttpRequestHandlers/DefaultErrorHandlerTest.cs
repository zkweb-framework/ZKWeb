using DotLiquid.Exceptions;
using System.Collections.Generic;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;
using System.Runtime.CompilerServices;

namespace ZKWeb.Tests.Web.HttpRequestHandlers {
	[Tests]
	class DefaultErrorHandlerTest {
		private class TestLogManager : LogManager {
			internal bool receivedLogError = false;

			public override void LogError(
				string message, string memberName, string filePath, int lineNumber) {
				receivedLogError = true;
			}
		}

		public void OnRequest() {
			var handler = new DefaultErrorHandler();
			using (Application.OverrideIoc()) {
				var logManagerMock = new TestLogManager();
				var config = new WebsiteConfig() { Extra = new Dictionary<string, object>() };
				var configManagerMock = new WebsiteConfigManager();
				configManagerMock.WebsiteConfig = config;
				Application.Ioc.Unregister<LogManager>();
				Application.Ioc.RegisterInstance<LogManager>(logManagerMock);
				Application.Ioc.Unregister<WebsiteConfigManager>();
				Application.Ioc.RegisterInstance<WebsiteConfigManager>(configManagerMock);
				// Only return message if error occurs with ajax request
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					request.headers["X-Requested-With"] = "XMLHttpRequest";
					handler.OnError(new ArgumentException("some message"));
					Assert.Equals(response.GetContentsFromBody(), "some message");
				}
				// Display status and message if the error is HttpException
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(404, "wrong address"));
					var contents = response.GetContentsFromBody();
					Assert.IsTrueWith(
						contents.Contains("404 wrong address") && !contents.Contains("<pre>"), contents);
				}
				// Display full exception dependent on website configuration
				config.Extra[ExtraConfigKeys.DisplayFullExceptionForRequest] = true;
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new ArgumentException("500 some error"));
					var contents = response.GetContentsFromBody();
					Assert.IsTrueWith(
						contents.Contains("500 some error") && contents.Contains("<pre>"), contents);
				}
				config.Extra[ExtraConfigKeys.DisplayFullExceptionForRequest] = false;
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new ArgumentException("500 some error"));
					var contents = response.GetContentsFromBody();
					Assert.IsTrueWith(
						contents.Contains("500 some error") && !contents.Contains("<pre>"), contents);
				}
				// Test error logging
				logManagerMock.receivedLogError = false;
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(401, "user error"));
					Assert.IsTrue(!logManagerMock.receivedLogError);
				}
				logManagerMock.receivedLogError = false;
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(500, "server error"));
					Assert.IsTrue(logManagerMock.receivedLogError);
				}
			}
		}
	}
}
