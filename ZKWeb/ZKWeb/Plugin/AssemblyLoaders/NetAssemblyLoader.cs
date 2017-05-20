#if !NETCORE
using System;
using System.Collections.Generic;
using System.Reflection;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Assembly loader for .Net Framework<br/>
	/// .Net Framework使用的程序集加载器<br/>
	/// </summary>
	internal class NetAssemblyLoader : AssemblyLoaderBase {
		/// <summary>
		/// Possible assembly name suffixes<br/>
		/// Use to load assemblies by short name<br/>
		/// 预置的程序集名称后续<br/>
		/// 用于支持根据短名称加载程序集<br/>
		/// </summary>
		private IList<string> PossibleAssemblyNameSuffixes { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public NetAssemblyLoader() {
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolver;
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			PossibleAssemblyNameSuffixes = new List<string>() {
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
			};
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				HandleLoadedAssembly(assembly);
			}
		}

		/// <summary>
		/// Load assembly by name<br/>
		/// 根据名称加载程序集<br/>
		/// </summary>
		public override Assembly Load(string name) {
			// Replace name if replacement exists
			name = ReplacementAssemblies.GetOrDefault(name, name);
			Assembly assembly = null;
			try {
				// Try load directly
				assembly = Load(new AssemblyName(name));
			} catch {
				// If load failed, add suffixes and try again
				foreach (var suffix in PossibleAssemblyNameSuffixes) {
					try {
						assembly = Load(new AssemblyName(name + suffix));
						break;
					} catch {
					}
				}
				if (assembly == null) {
					throw;
				}
			}
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly by name object<br/>
		/// 根据名称对象加载程序集<br/>
		/// </summary>
		public override Assembly Load(AssemblyName assemblyName) {
			var assembly = Assembly.Load(assemblyName);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly from it's binary contents<br/>
		/// 根据二进制内容加载程序集<br/>
		/// </summary>
		public override Assembly Load(byte[] rawAssembly) {
			var assembly = Assembly.Load(rawAssembly);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly from file path<br/>
		/// 根据文件路径加载程序集<br/>
		/// </summary>
		public override Assembly LoadFile(string path) {
			var assembly = Assembly.LoadFile(path);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Assembly resolve event handler<br/>
		/// 程序集解决事件的处理器<br/>
		/// </summary>
		/// <returns></returns>
		private Assembly AssemblyResolver(object sender, ResolveEventArgs args) {
			// If assembly already loaded, return the loaded instance
			var requireName = new AssemblyName(args.Name);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == requireName.Name) {
					return assembly;
				}
			}
			// Try to load Assembly from plugin's reference directory
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins) {
				var path = plugin.ReferenceAssemblyPath(requireName.Name);
				if (path != null) {
					return Assembly.LoadFrom(path);
				}
			}
			// It's not found, return null
			return null;
		}
	}
}
#endif
