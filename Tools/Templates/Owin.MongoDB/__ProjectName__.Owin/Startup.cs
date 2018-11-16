using Microsoft.Owin;
using ZKWeb.Hosting.Owin;

[assembly: OwinStartup(typeof(__ProjectName__.Owin.Startup))]
namespace __ProjectName__.Owin {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Startup : StartupBase { }
}
