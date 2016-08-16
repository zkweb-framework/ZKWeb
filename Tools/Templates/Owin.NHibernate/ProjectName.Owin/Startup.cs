using Microsoft.Owin;
using ZKWeb.Hosting.Owin;

[assembly: OwinStartup(typeof(${ProjectName}.Owin.Startup))]
namespace ${ProjectName}.Owin {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Startup : StartupBase { }
}
