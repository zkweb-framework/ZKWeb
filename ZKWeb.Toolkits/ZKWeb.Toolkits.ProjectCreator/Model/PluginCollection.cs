using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Toolkits.ProjectCreator.Model {
	/// <summary>
	/// 插件集合的定义
	/// plugin.collection.json的内容
	/// </summary>
	public class PluginCollection {
		/// <summary>
		/// 在插件列表前添加的插件
		/// </summary>
		public IList<string> PrependPlugins { get; set; }
		/// <summary>
		/// 在插件列表后添加的插件
		/// </summary>
		public IList<string> AppendPlugins { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public PluginCollection() {
			PrependPlugins = new List<string>();
			AppendPlugins = new List<string>();
		}

		/// <summary>
		/// 从文件读取插件集合
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <returns></returns>
		public static PluginCollection FromFile(string path) {
			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<PluginCollection>(json);
		}
	}
}
