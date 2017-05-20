namespace ZKWeb.Plugin {
	/// <summary>
	/// Interface used to perform processing after the plugin loads<br/>
	/// 用于在插件加载后执行处理的接口<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class Plugin : IPlugin {
	///		Plugin() { Console.WriteLine("plugin loaded"); }
	/// }
	/// </code>
	/// </example>
	public interface IPlugin { }
}
