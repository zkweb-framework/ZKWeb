using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZKWeb.Storage;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Plugin information<br/>
	/// 插件信息<br/>
	/// </summary>
	public class PluginInfo {
		/// <summary>
		/// Plugin directory<br/>
		/// 插件所在目录<br/>
		/// </summary>
		public string Directory { get; set; }
		/// <summary>
		/// Plugin name<br/>
		/// 插件名称<br/>
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Version string<br/>
		/// Format: "major.minor.build any string"<br/>
		/// 版本字符串<br/>
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// Plugin description<br/>
		/// 插件描述<br/>
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Dependent other plugins<br/>
		/// 依赖的其他插件<br/>
		/// </summary>
		public IList<string> Dependencies { get; set; }
		/// <summary>
		/// Reference assembly names<br/>
		/// 引用的程序集<br/>
		/// </summary>
		public IList<string> References { get; set; }
		/// <summary>
		/// Extra information<br/>
		/// 附加信息<br/>
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }
		/// <summary>
		/// Plugin assembly<br/>
		/// Maybe null if the plugin not contains dll or didn't loaded<br/>
		/// 插件程序集<br/>
		/// 如果插件未包含dll或者未加载, 这个成员会等于null<br/>
		/// </summary>
		[JsonIgnore]
		public Assembly Assembly { get; set; }

		/// <summary>
		/// Get plugin information from directory<br/>
		/// 从文件夹获取插件信息<br/>
		/// </summary>
		/// <param name="dir">Directory</param>
		/// <returns></returns>
		public static PluginInfo FromDirectory(string dir) {
			// Read plugin information from json
			// Create a default information instance if json file not exist
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var jsonPath = Path.Combine(dir, pathConfig.PluginInfoFilename);
			var json = File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : "{}";
			var info = JsonConvert.DeserializeObject<PluginInfo>(json);
			info.Directory = dir;
			info.Name = info.Name ?? Path.GetFileName(dir);
			info.Description = info.Description ?? info.Name;
			info.Version = info.Version ?? "0";
			info.Dependencies = info.Dependencies ?? new List<string>();
			info.References = info.References ?? new List<string>();
			info.Extra = info.Extra ?? new Dictionary<string, object>();
			return info;
		}
	}
}
