using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Web.Interfaces {
	/// <summary>
	/// 控制器的接口
	/// 例子
	///		[ExportMany]
	///		class TestController : IController {
	///			[Action("index.html")]
	///			public string Index() { return "test index"; }
	///		}
	/// </summary>
	public interface IController {
	}
}
