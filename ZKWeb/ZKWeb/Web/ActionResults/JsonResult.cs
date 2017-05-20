using Newtonsoft.Json;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Write json to response<br/>
	/// 写入Json到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			return new JsonResult(new { a = 100 });
	///		}
	///	}
	/// </code>
	/// </example>
	public class JsonResult : IActionResult {
		/// <summary>
		/// The object serialize to json<br/>
		/// 序列化到json的对象<br/>
		/// </summary>
		public object Object { get; set; }
		/// <summary>
		/// Serialize formatting<br/>
		/// 序列化格式<br/>
		/// </summary>
		public Formatting SerializeFormatting { get; set; }
		/// <summary>
		/// Content Type
		/// Default is "application/json; charset=utf-8"<br/>
		/// 内容类型<br/>
		/// 默认是"application/json; charset=utf-8"<br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="obj">The object serialize to json</param>
		/// <param name="formatting">Serialize formatting</param>
		public JsonResult(object obj, Formatting formatting = Formatting.None) {
			Object = obj;
			SerializeFormatting = formatting;
			ContentType = "application/json; charset=utf-8";
		}

		/// <summary>
		/// Write json to http response<br/>
		/// 写入json到http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = ContentType;
			// Write json to http response
			response.Write(JsonConvert.SerializeObject(Object, SerializeFormatting));
		}
	}
}
