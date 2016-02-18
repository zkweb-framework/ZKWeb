using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Plugin.Interfaces {
	/// <summary>
	/// 用于初始化插件的接口
	/// 例子
	///		[ExportMany]
	///		class Plugin : IPlugin {
	///			Plugin() { Console.WriteLine("plugin loaded"); }
	///		}
	/// </summary>
	public interface IPlugin {
	}
}
