using Microsoft.Owin;
using Owin;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Owin.Hosting;
using ZKWebStandard.Extensions;

[assembly: OwinStartup(typeof(ZKWeb.Owin.Startup))]
namespace ZKWeb.Owin {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Startup : StartupBase { }
}
