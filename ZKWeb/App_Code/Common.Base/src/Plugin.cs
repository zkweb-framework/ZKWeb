using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Manager;

namespace ZKWeb.App_Code.Common.Base.src {
	/// <summary>
	/// 初始化插件
	/// </summary>
	public class Plugin {
		public Plugin() {
			Application.Current.Ioc.Resolve<LogManager>().LogDebug("plugin loaded");
		}
	}
}
