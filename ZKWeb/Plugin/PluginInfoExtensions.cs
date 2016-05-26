using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Plugin {
	/// <summary>
	/// 插件信息的扩展函数
	/// </summary>
	public static class PluginInfoExtensions {
		/// <summary>
		/// 编译插件时的默认引用程序集
		/// </summary>
		public static List<string> DefaultReferences { get; } =
			new List<string>() { "NHibernate", "FluentNHibernate", "DotLiquid" };

		/// <summary>
		/// 编译插件
		/// </summary>
		/// <param name="info">插件信息</param>
		public static void Compile(this PluginInfo info) {
			// 获取插件的源代码文件列表和各个路径
			var sourceDirectory = info.SourceDirectory();
			var sourceFiles = info.SourceFiles();
			var assemblyPath = info.AssemblyPath();
			var assemblyPdbPath = info.AssemblyPdbPath();
			var compileInfoPath = info.CompileInfoPath();
			// 检查是否需要重新编译
			// 会通过对比所有源文件的修改时间是否一致来检查
			// 没有源文件时表示只有资源文件或不开源，不需要重新编译
			var existCompileInfo = "";
			if (File.Exists(compileInfoPath)) {
				existCompileInfo = File.ReadAllText(compileInfoPath);
			}
			var compileInfo = string.Join("\r\n", sourceFiles
				.Select(s => new {
					path = s.Substring(sourceDirectory.Length + 1),
					time = File.GetLastWriteTime(s)
				}) // 相对路径和修改时间
				.OrderBy(s => s.path) // 固定排序
				.Select(s => $"{s.path} {s.time}")); // 生成文本
			if (sourceFiles.Length > 0 && compileInfo != existCompileInfo) {
				// 重新编译前把原来的文件重命名为old文件
				if (File.Exists(assemblyPath)) {
					File.Move(assemblyPath, $"{assemblyPath}.{DateTime.UtcNow.Ticks}.old");
				}
				if (File.Exists(assemblyPdbPath)) {
					File.Move(assemblyPdbPath, $"{assemblyPdbPath}.{DateTime.UtcNow.Ticks}.old");
				}
				// 编译，debug = true可以生成pdb文件以支持断点
				var references = info.References.Concat(DefaultReferences);
				var referenceLocations = references.Select(
					r => Assembly.Load(r).Location).ToArray();
				Directory.CreateDirectory(info.BinDirectory());
				CSScript.CompileFiles(sourceFiles, assemblyPath, true, referenceLocations);
				// 保存编译信息
				File.WriteAllText(compileInfoPath, compileInfo);
				// 删除old文件
				// 有可能因为文件占用而删除不成功，忽略删除失败时的例外
				Directory.EnumerateFiles(info.BinDirectory(), "*.old").ForEach(
					path => { try { File.Delete(path); } catch { } });
			}
		}

		/// <summary>
		/// 获取插件目录的名称
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string DirectoryName(this PluginInfo info) {
			return Path.GetFileName(info.Directory);
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
			return Path.Combine(info.BinDirectory(), $"{info.DirectoryName()}.dll");
		}

		/// <summary>
		/// 获取插件程序集对应的pdb文件路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string AssemblyPdbPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), $"{info.DirectoryName()}.pdb");
		}

		/// <summary>
		/// 获取保存编译信息的文件路径
		/// 这个文件中有源代码和修改时间的列表
		/// 用于检测是否需要重新编译
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string CompileInfoPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), "CompileInfo.txt");
		}

		/// <summary>
		/// 获取版本对象
		/// 解析失败时返回默认的版本对象，不抛出例外
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static Version VersionObject(this PluginInfo info) {
			Version version;
			if (Version.TryParse(info.Version.Split(' ')[0], out version)) {
				return version;
			}
			return new Version();
		}
	}
}
