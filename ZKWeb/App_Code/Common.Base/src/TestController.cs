using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Manager;
using ZKWeb.Model;
using ZKWeb.Model.ActionResults;

namespace ZKWeb.App_Code.Common.Base.src {
	public class TestController {
		[Action("/")]
		public string Index() {
			return "test index";
		}

		[Action("static")]
		public static string Static() {
			return "test static";
		}

		[Action("abc")]
		public IActionResult Abc() {
			return new PlainResult(123);
		}
	}
}
