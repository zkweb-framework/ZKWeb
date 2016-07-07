#if NETCORE
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// .Net Core使用的程序集载入器
	/// </summary>
	internal class CoreAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// 载入程序集使用的上下文
		/// </summary>
		private LoadContext Context { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public CoreAssemblyLoader() {
			Context = new LoadContext();
		}

		/// <summary>
		/// 获取当前已载入的程序集列表
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return DependencyContext.Default.RuntimeLibraries
				.SelectMany(l => l.GetDefaultAssemblyNames(DependencyContext.Default))
				.Select(name => Context.LoadFromAssemblyName(name)).ToList();
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(string name) {
			return Context.LoadFromAssemblyName(new AssemblyName(name));
		}

		/// <summary>
		/// 从二进制数据载入程序集
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				return Context.LoadFromStream(stream);
			}
		}

		/// <summary>
		/// 从文件载入程序集
		/// </summary>
		public Assembly LoadFile(string path) {
			return Context.LoadFromAssemblyPath(path);
		}

		/// <summary>
		/// 载入程序集使用的上下文
		/// 参考
		/// https://github.com/dotnet/roslyn/blob/master/src/Scripting/Core/Hosting/AssemblyLoader/CoreAssemblyLoaderImpl.cs
		/// </summary>
		private class LoadContext : AssemblyLoadContext {
			protected override Assembly Load(AssemblyName assemblyName) {
				return Default.LoadFromAssemblyName(assemblyName);
			}
		}
	}
}
#endif
