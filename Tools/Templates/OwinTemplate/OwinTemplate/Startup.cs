using Microsoft.Owin;
using ZKWeb.Hosting.Owin;

[assembly: OwinStartup(typeof(${ProjectName}.Startup))]
namespace ${ProjectName} {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Startup : StartupBase { }
}
