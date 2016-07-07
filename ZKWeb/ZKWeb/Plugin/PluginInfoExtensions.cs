using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Plugin.CompilerServices;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin {
	/// <summary>
	/// 插件信息的扩展函数
	/// </summary>
	public static class PluginInfoExtensions {
		/// <summary>
		/// 插件目录的名称
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string DirectoryName(this PluginInfo info) {
			return Path.GetFileName(info.Directory);
		}

		/// <summary>
		/// 插件源文件的目录路径
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string SourceDirectory(this PluginInfo info) {
			return Path.Combine(info.Directory, "src");
		}

		/// <summary>
		/// 插件程序集的目录路径
		/// 路径中带有当前编译到的平台名称
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		public static string BinDirectory(this PluginInfo info) {
			var compilerService = Application.Ioc.Resolve<ICompilerService>();
			return Path.Combine(info.Directory, "bin", compilerService.TargetPlatform);
		}

		/// <summary>
		/// 插件引用程序集的目录路径
		/// 路径中带有当前编译到的平台名称
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <returns></returns>
		[Obsolete("Please use ReferenceAssemblyPath")]
		public static string ReferencesDirectory(this PluginInfo info) {
			var compilerService = Application.Ioc.Resolve<ICompilerService>();
			return Path.Combine(info.Directory, "references", compilerService.TargetPlatform);
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
		/// 获取插件引用程序集的目录路径
		/// 获取失败时抛出例外
		/// </summary>
		/// <param name="info">插件信息</param>
		/// <param name="assemblyName">引用的程序集名称</param>
		/// <returns></returns>
		public static string ReferenceAssemblyPath(this PluginInfo info, string assemblyName) {
			var compilerService = Application.Ioc.Resolve<ICompilerService>();
			var platform = compilerService.TargetPlatform;
			var paths = new List<string>() {
				Path.Combine(info.Directory, "references", platform, $"{assemblyName}.dll"),
				Path.Combine(info.Directory, "references", $"{assemblyName}.dll")
			};
			foreach (var path in paths) {
				if (File.Exists(path)) {
					return path;
				}
			}
			throw new FileNotFoundException($"Reference assembly {assemblyName} path not found");
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
			return new Version(0, 0, 0);
		}

		/// <summary>
		/// 编译插件
		/// </summary>
		/// <param name="info">插件信息</param>
		public static void Compile(this PluginInfo info) {
			// 获取插件的源代码文件列表和各个路径
			var sourceDirectory = info.SourceDirectory();
			var sourceFiles = info.SourceFiles();
			var assemblyName = info.DirectoryName();
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
				// 调用编译器编译
				Directory.CreateDirectory(Path.GetDirectoryName(assemblyPath));
				var compilerService = Application.Ioc.Resolve<ICompilerService>();
				var options = new CompilationOptions();
				options.ReferenceAssemblyPaths.AddRange(
					info.References.Select(name => ReferenceAssemblyPath(info, name)));
				compilerService.Compile(sourceFiles, assemblyName, assemblyPath, options);
				// 保存编译信息
				File.WriteAllText(compileInfoPath, compileInfo);
				// 删除old文件
				// 有可能因为文件占用而删除不成功，忽略删除失败时的例外
				Directory.EnumerateFiles(info.BinDirectory(), "*.old")
					.ForEach(path => { try { File.Delete(path); } catch { } });
			}
		}
	}
}
