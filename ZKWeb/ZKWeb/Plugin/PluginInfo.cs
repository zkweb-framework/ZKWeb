using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZKWeb.Storage;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Plugin information
	/// </summary>
	public class PluginInfo {
		/// <summary>
		/// Plugin directory
		/// </summary>
		public string Directory { get; set; }
		/// <summary>
		/// Plugin name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Version string
		/// Format: "major.minor.build any string"
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// Plugin description
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Dependent other plugins
		/// </summary>
		public IList<string> Dependencies { get; set; }
		/// <summary>
		/// Reference assembly names
		/// </summary>
		public IList<string> References { get; set; }
		/// <summary>
		/// Extra information
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }
		/// <summary>
		/// Plugin assembly
		/// Maybe null if plugin not contains dll or didn't loaded
		/// </summary>
		[JsonIgnore]
		public Assembly Assembly { get; set; }

		/// <summary>
		/// Get plugin information from directory
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
