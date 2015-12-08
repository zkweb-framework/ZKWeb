using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Manager;
using ZKWeb.Model;

namespace ZKWeb.App_Code.Common.Base.src {
	/// <summary>
	/// 初始化插件
	/// </summary>
	public class Plugin {
		public Plugin() {
			Application.Ioc.RegisterMany<ApplicationRequestHandler>();
		}
	}

	public class ApplicationRequestHandler : IApplicationRequestHandler {
		public void OnRequest() {
			var context = HttpContext.Current;
			context.Response.Write("hello world path = " + context.Request.Path);
			context.Response.End();
		}
	}
}
