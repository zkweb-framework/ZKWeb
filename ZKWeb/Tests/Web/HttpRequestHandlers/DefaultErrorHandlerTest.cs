using DotLiquid.Exceptions;
using NSubstitute;
using System.Collections.Generic;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.HttpRequestHandlers {
	[Tests]
	class DefaultErrorHandlerTest {
		public void OnRequest() {
			var handler = new DefaultErrorHandler();
			using (Application.OverrideIoc()) {
				var logManagerMock = Substitute.For<LogManager>();
				var config = new WebsiteConfig() { Extra = new Dictionary<string, object>() };
				var configManagerMock = Substitute.For<ConfigManager>();
				configManagerMock.WebsiteConfig.Returns(config);
				Application.Ioc.Unregister<LogManager>();
				Application.Ioc.RegisterInstance(logManagerMock);
				Application.Ioc.Unregister<ConfigManager>();
				Application.Ioc.RegisterInstance(configManagerMock);
				// Ajax请求时只显示消息
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					request.headers["X-Requested-With"] = "XMLHttpRequest";
					handler.OnError(new ArgumentException("some message"));
					Assert.Equals(response.GetContentsFromBody(), "some message");
				}
				// HttpException时显示状态代码和消息
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(404, "wrong address"));
					Assert.Equals(response.GetContentsFromBody(), "<h1>404 wrong address</h1>");
				}
				// 根据网站配置决定是否显示完整信息
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
				// 测试记录日志
				logManagerMock.ClearReceivedCalls();
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(401, "user error"));
					logManagerMock.DidNotReceiveWithAnyArgs().LogError(null);
				}
				logManagerMock.ClearReceivedCalls();
				using (HttpManager.OverrideContext("", "GET")) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					handler.OnError(new HttpException(500, "server error"));
					logManagerMock.ReceivedWithAnyArgs().LogError(null);
				}
			}
		}
	}
}
