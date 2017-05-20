using System;
using System.Text;
using ZKWeb.Logging;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// Default error handler<br/>
	/// 默认的错误处理器<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	public class DefaultErrorHandler : IHttpRequestErrorHandler {
		/// <summary>
		/// Handler request error<br/>
		/// 处理请求错误<br/>
		/// </summary>
		public void OnError(Exception ex) {
			// Use most inner exception
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			// Set status
			var response = HttpManager.CurrentContext.Response;
			var httpExcepion = ex as HttpException;
			var statusCode = httpExcepion?.StatusCode ?? 500;
			// If error ain't client error, log it
			if (!(statusCode >= 400 && statusCode < 500)) {
				var logManager = Application.Ioc.Resolve<LogManager>();
				logManager.LogError(ex.ToDetailedString());
			}
			// Check if it's ajax request
			var request = HttpManager.CurrentContext.Request;
			response.StatusCode = statusCode;
			if (request.IsAjaxRequest()) {
				// Only return message for ajax request
				response.ContentType = "text/plain";
				response.Write(ex.ToSummaryString());
				response.End();
			} else {
				// Return staatus and message for other request
				// If display full exception is allowed, return full exception information
				var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
				var displayFullException = (configManager.WebsiteConfig
					.Extra.GetOrDefault<bool?>(ExtraConfigKeys.DisplayFullExceptionForRequest) ?? true);
				var message = new StringBuilder();
				message.Append("<head><meta charset='utf-8' /></head>");
				message.AppendFormat("<h1>{0} {1}</h1>",
					statusCode, HttpUtils.HtmlEncode(ex.Message));
				if (displayFullException && httpExcepion == null) {
					message.AppendFormat("<pre>{0}</pre>", HttpUtils.HtmlEncode(ex.ToDetailedString()));
				}
				response.ContentType = "text/html";
				response.Write(message.ToString());
				response.End();
			}
		}
	}
}
