using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Toolkits.ProjectCreator.Model {
	/// <summary>
	/// Plugin collection
	/// Deserialize from `plugin.collection.json`
	/// </summary>
	public class PluginCollection {
		/// <summary>
		/// Plugins prepend to plugin list
		/// </summary>
		public IList<string> PrependPlugins { get; set; }
		/// <summary>
		/// Plugins append to plugin list
		/// </summary>
		public IList<string> AppendPlugins { get; set; }
		/// <summary>
		/// Supported ORM list
		/// </summary>
		public IList<string> SupportedORM { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public PluginCollection() {
			PrependPlugins = new List<string>();
			AppendPlugins = new List<string>();
			SupportedORM = new List<string>();
		}

		/// <summary>
		/// Read plugin collection from file
		/// </summary>
		/// <param name="path">Json file path</param>
		/// <returns></returns>
		public static PluginCollection FromFile(string path) {
			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<PluginCollection>(json);
		}
	}
}
