using DryIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Core;
using ZKWeb.Model;
using ZKWeb.Model.ActionResults;

namespace ZKWeb.App_Code.Common.Base.src {
	public class AdminController {
		[Action("admin")]
		[Action("admin.html")]
		[Action("admin.aspx")]
		public string Admin() {
			return "here is admin";
		}

		[Action("admin/info")]
		public IActionResult Info() {
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			return new JsonResult(new {
				Plugins = pluginManager.Plugins
			}, Formatting.Indented);
		}
	}
}