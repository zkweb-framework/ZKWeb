using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Model.ActionResults {
	/// <summary>
	/// Json结果
	/// </summary>
	public class JsonResult : IActionResult {
		/// <summary>
		/// 写入的对象
		/// </summary>
		public object Object { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="obj"></param>
		public JsonResult(object obj) {
			Object = obj;
		}

		/// <summary>
		/// 写入json到http回应中
		/// </summary>
		/// <param name="response"></param>
		public void WriteResponse(HttpResponse response) {
			response.ContentType = "application/json";
			response.Write(JsonConvert.SerializeObject(Object));
		}
	}
}
