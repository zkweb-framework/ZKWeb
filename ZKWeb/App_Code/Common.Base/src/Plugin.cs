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
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			controllerManager.RegisterController<TestController>();
			controllerManager.RegisterController<AdminController>();
		}
	}
}
