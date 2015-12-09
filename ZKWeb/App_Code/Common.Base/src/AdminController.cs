using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Core;

namespace ZKWeb.App_Code.Common.Base.src {
	public class AdminController {
		[Action("admin")]
		[Action("admin.html")]
		[Action("admin.aspx")]
		public string Admin() {
			return "here is admin";
		}
	}
}