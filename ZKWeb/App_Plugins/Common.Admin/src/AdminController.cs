using DryIoc;
using DryIocAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb;
using ZKWeb.Core;
using ZKWeb.Model;
using ZKWeb.Model.ActionResults;

namespace Common.Admin {
	[ExportMany]
	public class AdminController : IController {
		[Action("admin")]
		[Action("admin.html")]
		[Action("admin.aspx")]
		public string Admin() {
			return "here is admin\r\n" + new Base.Database.Session().ToString();
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
