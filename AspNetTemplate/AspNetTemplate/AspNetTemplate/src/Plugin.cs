using ZKWeb.Plugin;
using ZKWebStandard.Ioc;

namespace AspNetTemplate.AspNetTemplate.src {
	/// <summary>
	/// Plugin Entry Point
	/// </summary>
	[ExportMany]
	public class Plugin : IPlugin {
		/// <summary>
		/// Here will execute after plugin loaded
		/// </summary>
		public Plugin() { }
	}
}
