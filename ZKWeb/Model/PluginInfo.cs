using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ZKWeb.Model {
	/// <summary>
	/// 插件信息
	/// </summary>
	public class PluginInfo {
		/// <summary>
		/// 插件所在目录
		/// </summary>
		public string Directory { get; set; }
		/// <summary>
		/// 插件名称
		/// </summary>
		public Dictionary<string, string> Name { get; set; }
		/// <summary>
		/// 插件描述
		/// </summary>
		public Dictionary<string, string> Description { get; set; }
		/// <summary>
		/// 依赖的其他插件
		/// </summary>
		public string[] Dependencies { get; set; }
		/// <summary>
		/// 依赖的程序集列表
		/// </summary>
		public string[] References { get; set; }

		/// <summary>
		/// 从插件目录生成插件信息
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static PluginInfo FromDirectory(string dir) {
			// 从json文件中读取插件信息，不存在时生成空的信息
			var jsonPath = Path.Combine(dir, PathConfig.PluginInfoFilename);
			var json = File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : "{}";
			var info = JsonConvert.DeserializeObject<PluginInfo>(json);
			info.Directory = dir;
			info.Name = info.Name ?? new Dictionary<string, string>();
			info.Description = info.Description ?? new Dictionary<string, string>();
			info.Dependencies = info.Dependencies ?? new string[0];
			info.References = info.References ?? new string[0];
			return info;
		}
	}
}
