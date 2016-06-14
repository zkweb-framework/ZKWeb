using Newtonsoft.Json;
using ZKWeb.Web.Abstractions;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Json结果
	/// </summary>
	public class JsonResult : IActionResult {
		/// <summary>
		/// 写入的对象
		/// </summary>
		public object Object { get; set; }
		/// <summary>
		/// 序列化时使用的格式
		/// </summary>
		public Formatting SerializeFormatting { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="obj">写入的对象</param>
		/// <param name="formatting">序列化时使用的格式</param>
		public JsonResult(object obj, Formatting formatting = Formatting.None) {
			Object = obj;
			SerializeFormatting = formatting;
		}

		/// <summary>
		/// 写入Json到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// 设置状态代码和内容类型
			response.StatusCode = 200;
			response.ContentType = "application/json";
			// 写入Json到Http回应
			response.Write(JsonConvert.SerializeObject(Object, SerializeFormatting));
		}
	}
}
