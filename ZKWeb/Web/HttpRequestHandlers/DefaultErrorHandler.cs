using System;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWeb.Web;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// 默认的请求错误处理器
	/// </summary>
	public class DefaultErrorHandler : IHttpRequestErrorHandler {
		/// <summary>
		/// 处理请求错误
		/// </summary>
		public void OnError(Exception ex) {
			// 设置错误代码到回应
			var response = HttpManager.CurrentContext.Response;
			var httpExcepion = ex as HttpException;
			var statusCode = httpExcepion?.StatusCode ?? 500;
			// 记录除了客户端错误以外的错误到日志
			if (!(statusCode >= 400 && statusCode < 500)) {
				var logManager = Application.Ioc.Resolve<LogManager>();
				logManager.LogError(ex.ToString());
			}
			// 根据请求类型写入到回应
			var request = HttpManager.CurrentContext.Request;
			response.StatusCode = statusCode;
			if (request.IsAjaxRequest()) {
				// Ajax请求时只返回例外消息
				response.ContentType = "text/plain";
				response.Write(ex.Message);
				response.End();
			} else {
				// 其他请求时
				// - 判断是否HttpException，如果是则只显示例外消息
				// - 判断网站配置中是否允许显示详细错误，如果允许则显示详细错误
				var configManager = Application.Ioc.Resolve<ConfigManager>();
				var displayFullException = (configManager.WebsiteConfig
					.Extra.GetOrDefault<bool?>(ExtraConfigKeys.DisplayFullExceptionForRequest) ?? true);
				var message = string.Format("<h1>{0} {1}</h1>",
					statusCode, HttpUtils.HtmlEncode(ex.Message));
				if (displayFullException && httpExcepion == null) {
					message += string.Format($"<br /><br /><pre>{0}</pre>", HttpUtils.HtmlEncode(ex));
				}
				response.ContentType = "text/html";
				response.Write(message);
				response.End();
			}
		}
	}
}
