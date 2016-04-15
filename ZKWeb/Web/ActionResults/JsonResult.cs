using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Web.Interfaces;

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
		/// 写入json到http回应中
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(HttpResponse response) {
			response.ContentType = "application/json";
			response.Write(JsonConvert.SerializeObject(Object, SerializeFormatting));
		}
	}
}
