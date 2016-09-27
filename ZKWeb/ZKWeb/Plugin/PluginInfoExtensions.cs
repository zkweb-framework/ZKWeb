using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Plugin.CompilerServices;
using ZKWeb.Server;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Plugin information extension methods
	/// </summary>
	public static class PluginInfoExtensions {
		/// <summary>
		/// Get directory name
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string DirectoryName(this PluginInfo info) {
			return Path.GetFileName(info.Directory);
		}

		/// <summary>
		/// Get source directory
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string SourceDirectory(this PluginInfo info) {
			return Path.Combine(info.Directory, "src");
		}

		/// <summary>
		/// Get binary directory
		/// Path contains target platform name
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string BinDirectory(this PluginInfo info) {
			var compilerService = Application.Ioc.Resolve<ICompilerService>();
			return Path.Combine(info.Directory, "bin", compilerService.TargetPlatform);
		}

		/// <summary>
		/// Get source files 
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string[] SourceFiles(this PluginInfo info) {
			var sourceDirectory = info.SourceDirectory();
			if (Directory.Exists(sourceDirectory)) {
				return Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories).ToArray();
			}
			return new string[0];
		}

		/// <summary>
		/// Get assembly file path
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string AssemblyPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), $"{info.DirectoryName()}.dll");
		}

		/// <summary>
		/// Get pdb file path
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string AssemblyPdbPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), $"{info.DirectoryName()}.pdb");
		}

		/// <summary>
		/// Get compile infomation path
		/// Contains source code paths and it's modify time,
		/// use to determine if recompile is needed
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static string CompileInfoPath(this PluginInfo info) {
			return Path.Combine(info.BinDirectory(), "CompileInfo.txt");
		}

		/// <summary>
		/// Get assembly path from plugin's reference directory
		/// Return null if not found
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <param name="assemblyName">Assembly name</param>
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
			return null;
		}

		/// <summary>
		/// Get plugin version objects
		/// Return an empty version if parse failed
		/// </summary>
		/// <param name="info">Plugin information</param>
		/// <returns></returns>
		public static Version VersionObject(this PluginInfo info) {
			Version version;
			if (Version.TryParse(info.Version.Split(' ')[0], out version)) {
				return version;
			}
			return new Version(0, 0, 0);
		}

		/// <summary>
		/// Compile plugin
		/// </summary>
		/// <param name="info">Plugin information</param>
		public static void Compile(this PluginInfo info) {
			// Get source files and releated paths
			var sourceDirectory = info.SourceDirectory();
			var sourceFiles = info.SourceFiles();
			var assemblyName = info.DirectoryName();
			var assemblyPath = info.AssemblyPath();
			var assemblyPdbPath = info.AssemblyPdbPath();
			var compileInfoPath = info.CompileInfoPath();
			// Load reference assemblies
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var reference in info.References) {
				assemblyLoader.Load(reference);
			}
			// Check if recompile is needed
			// If no source files exists then no need to compile
			var existCompileInfo = "";
			if (File.Exists(compileInfoPath)) {
				existCompileInfo = File.ReadAllText(compileInfoPath);
			}
			var compileInfo = string.Join("\r\n", sourceFiles
				.Select(s => new {
					path = s.Substring(sourceDirectory.Length + 1),
					time = File.GetLastWriteTime(s)
				}) // Relative path and modify time
				.OrderBy(s => s.path) // Order by path
				.Select(s => $"{s.path} {s.time}")); // Generate line
			if (sourceFiles.Length > 0 && compileInfo != existCompileInfo) {
				// Rename old files
				if (File.Exists(assemblyPath)) {
					File.Move(assemblyPath, $"{assemblyPath}.{DateTime.UtcNow.Ticks}.old");
				}
				if (File.Exists(assemblyPdbPath)) {
					File.Move(assemblyPdbPath, $"{assemblyPdbPath}.{DateTime.UtcNow.Ticks}.old");
				}
				// Invoke compile service
				// Default use debug configuration
				Directory.CreateDirectory(Path.GetDirectoryName(assemblyPath));
				var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
				var release = configManager.WebsiteConfig.Extra.GetOrDefault<bool?>(
					ExtraConfigKeys.CompilePluginsWithReleaseConfiguration) ?? false;
				var compilerService = Application.Ioc.Resolve<ICompilerService>();
				var options = new CompilationOptions();
				options.Release = release;
				options.GeneratePdbFile = true;
				compilerService.Compile(sourceFiles, assemblyName, assemblyPath, options);
				// Write compile information
				File.WriteAllText(compileInfoPath, compileInfo);
				// Remove old files, maybe they are locking but that's not matter
				Directory.EnumerateFiles(info.BinDirectory(), "*.old")
					.ForEach(path => { try { File.Delete(path); } catch { } });
			}
		}
	}
}
