using Microsoft.Owin;
using ZKWeb.Hosting.Owin;

[assembly: OwinStartup(typeof(OwinTemplate.Startup))]
namespace OwinTemplate {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Startup : StartupBase { }
}
