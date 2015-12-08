using CSScriptLibrary;
using DryIoc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using ZKWeb.Core.Model;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Manager {
	/// <summary>
	/// 插件管理器
	/// 载入插件的流程
	///		枚举配置文件中的Plugins
	///		载入Plugins.json中的插件信息
	///		使用Csscript编译插件目录下的源代码到dll
	///		载入编译好的dll并查找Plugin类名
	///		创建这个类型的对象以初始化插件
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// 插件列表
		/// </summary>
		public List<PluginInfo> Plugins { get; private set; } = new List<PluginInfo>();

		/// <summary>
		/// 载入所有插件
		/// </summary>
		public PluginManager() {
			// 获取网站配置中的插件列表
			var configManager = Application.Current.Ioc.Resolve<ConfigManager>();
			// 载入所有插件信息
			foreach (var pluginName in configManager.WebsiteConfig.Plugins) {
				var dir = PathUtils.SecureCombine(PathConfig.PluginsRootDirectory, pluginName);
				var info = PluginInfo.FromDirectory(dir);
				Plugins.Add(info);
			}
			// 注册解决程序集依赖的函数
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			// 载入插件
			foreach (var plugin in Plugins) {
				// 编译带源代码的插件
				plugin.Compile();
				// 载入插件程序集，注意部分插件只有资源文件没有程序集
				// 查找类名为Plugin的类并生成实例
				var assemblyPath = plugin.AssemblyPath();
				if (File.Exists(assemblyPath)) {
					var assembly = Assembly.LoadFile(assemblyPath);
					var pluginType = assembly.GetTypes().First(p => p.Name == "Plugin");
					Activator.CreateInstance(pluginType);
				}
			}
		}

		/// <summary>
		/// 程序集解决器
		/// 从插件目录下搜索程序集并载入
		/// </summary>
		/// <returns></returns>
		public Assembly AssemblyResolver(object sender, ResolveEventArgs args) {
			// 从已载入的程序集中查找
			var requireName = new AssemblyName(args.Name);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == requireName.Name) {
					return assembly;
				}
			}
			// 查找插件的引用程序集目录
			foreach (var plugin in Plugins) {
				var path = Path.Combine(plugin.ReferencesDirectory(), $"{requireName.Name}.dll");
				if (File.Exists(path)) {
					return Assembly.LoadFrom(path);
				}
			}
			// 找不到时返回null
			return null;
		}
	}

	/// <summary>
	/// 插件信息的扩展函数
	/// </summary>
	public static class PluginInfoExtensions {
		/// <summary>
		/// 编译插件
		/// </summary>
		/// <param name="info">插件信息</param>
		public static void Compile(this PluginInfo info) {
			// 检查是否需要重新编译
			// 没有源文件时表示可能是只有资源文件或不开源的插件，这时不需要重新编译
			// TODO 比较源文件的修改时间
			var sourceFiles = info.SourceFiles();
			var assemblyPath = info.AssemblyPath();
			var assemblyPdbPath = info.AssemblyPdbPath();
			if (sourceFiles.Length > 0) {
				// 重新编译前把原来的文件重命名为old文件
				if (File.Exists(assemblyPath)) {
					File.Move(assemblyPath, $"{assemblyPath}.{DateTime.UtcNow.Ticks}.old");
				}
				if (File.Exists(assemblyPdbPath)) {
					File.Move(assemblyPdbPath, $"{assemblyPdbPath}.{DateTime.UtcNow.Ticks}.old");
				}
				// 编译，debug = true可以生成pdb文件以支持断点
				Directory.CreateDirectory(info.BinDirectory());
				CSScript.CompileFiles(sourceFiles, assemblyPath, true, info.References);
				// 删除old文件
				// 有可能因为文件占用而删除不成功，忽略删除失败时的例外
				Directory.EnumerateFiles(info.BinDirectory(), "*.old").ForEach(
					path => { try { File.Delete(path); } catch { } });
			}
		}

		/// <summary>
		/// 获取插件源文件的目录路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string SourceDirectory(this PluginInfo info) {
			return Path.Combine(info.Directory, "src");
		}

		/// <summary>
		/// 获取插件程序集的目录路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string BinDirectory(this PluginInfo info) {
			return Path.Combine(info.Directory, "bin");
		}

		/// <summary>
		/// 获取插件引用程序集的目录路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string ReferencesDirectory(this PluginInfo info) {
			return Path.Combine(info.Directory, "references");
		}

		/// <summary>
		/// 获取插件的源文件列表 
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string[] SourceFiles(this PluginInfo info) {
			var sourceDirectory = info.SourceDirectory();
			if (Directory.Exists(sourceDirectory)) {
				return Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories).ToArray();
			}
			return new string[0];
		}

		/// <summary>
		/// 获取插件的程序集路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string AssemblyPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), $"{Path.GetFileName(info.Directory)}.dll");
		}

		/// <summary>
		/// 获取插件程序集对应的pdb文件路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string AssemblyPdbPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), $"{Path.GetFileName(info.Directory)}.pdb");
		}
	}
}
