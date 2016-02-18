using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;

namespace ZKWeb.Plugin {
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
		public string Name { get; set; }
		/// <summary>
		/// 插件描述
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 依赖的其他插件
		/// </summary>
		public List<string> Dependencies { get; set; }
		/// <summary>
		/// 引用的程序集列表
		/// </summary>
		public List<string> References { get; set; }

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
			info.Name = info.Name ?? Path.GetFileName(dir);
			info.Description = info.Description ?? info.Name;
			info.Dependencies = info.Dependencies ?? new List<string>();
			info.References = info.References ?? new List<string>();
			return info;
		}
	}
}
