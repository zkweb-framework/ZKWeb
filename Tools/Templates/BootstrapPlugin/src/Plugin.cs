using ${ProjectName}.Plugins.${ProjectName}.src.Controllers;
using System.Linq;
using ZKWeb;
using ZKWeb.Plugin;
using ZKWeb.Web;
using ZKWebStandard.Ioc;

namespace ${ProjectName}.Plugins.${ProjectName}.src {
	/// <summary>
	/// Plugin Entry Point
	/// </summary>
	[ExportMany, SingletonReuse]
	public class Plugin : IPlugin {
		/// <summary>
		/// Here will execute after plugin loaded
		/// </summary>
		public Plugin() {
			// Register hello controller if default plugin collections are not used
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			if (!pluginManager.Plugins.Any(p => p.DirectoryName() == "Common.Base")) {
				controllerManager.RegisterController(new HelloController());
				Application.Ioc.RegisterMany<HelloStaticHandler>(ReuseType.Singleton);
			}
		}
	}
}
