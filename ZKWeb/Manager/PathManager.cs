using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Manager {
	/// <summary>
	/// 路径管理器
	/// </summary>
	public class PathManager {
	}

	/// <summary>
	/// 路径的默认配置
	/// </summary>
	public static class PathConfig {
		/// <summary>
		/// App_Data目录
		/// </summary>
		public static string AppDataDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data"); }
		}

		/// <summary>
		/// 插件根目录
		/// </summary>
		public static string PluginsRootDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Code"); }
		}

		/// <summary>
		/// 数据文件根目录
		/// </summary>
		public static string StorageRootDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "Storage"); }
		}

		/// <summary>
		/// 日志文件目录
		/// </summary>
		public static string LogsDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "Logs"); }
		}

		/// <summary>
		/// 网站配置文件路径
		/// </summary>
		public static string WebsiteConfigPath {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "config.json"); }
		}
	}
}
