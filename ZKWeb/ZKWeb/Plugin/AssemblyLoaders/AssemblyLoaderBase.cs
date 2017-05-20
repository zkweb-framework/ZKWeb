using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Base class of assembly loader<br/>
	/// 程序集加载器的基类<br/>
	/// </summary>
	internal abstract class AssemblyLoaderBase : IAssemblyLoader {
		/// <summary>
		/// Replacement assemblies<br/>
		/// 程序集的替代设置<br/>
		/// </summary>
		protected IDictionary<string, string> ReplacementAssemblies { get; set; }
		/// <summary>
		/// Loaded assemblies<br/>
		/// 已加载的程序集<br/>
		/// </summary>
		protected virtual ISet<Assembly> LoadedAssemblies { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public AssemblyLoaderBase() {
			ReplacementAssemblies = new Dictionary<string, string>() {
				{ "System.FastReflection", "FastReflection" },
				{ "System.DrawingCore", "ZKWeb.System.Drawing" }
			};
			LoadedAssemblies = new HashSet<Assembly>();
		}

		/// <summary>
		/// Handle loaded assembly<br/>
		/// Preload it's dependencies<br/>
		/// 处理已加载的程序集<br/>
		/// 预加载它的引用项<br/>
		/// </summary>
		/// <param name="assembly">Assembly</param>
		/// <returns></returns>
		protected virtual Assembly HandleLoadedAssembly(Assembly assembly) {
			if (LoadedAssemblies.Contains(assembly)) {
				return assembly;
			}
			LoadedAssemblies.Add(assembly);
			// preload it's dependencies
			foreach (var dependentAssemblyname in assembly.GetReferencedAssemblies()) {
				Assembly dependentAssembly;
				try {
					dependentAssembly = Load(dependentAssemblyname);
				} catch {
					// may fail, ignore it
					continue;
				}
				HandleLoadedAssembly(dependentAssembly);
			}
			return assembly;
		}

		/// <summary>
		/// Get loaded assemblies<br/>
		/// Except wrapper assemblies and dynamic assemblies<br/>
		/// 获取已加载的程序集<br/>
		/// 排除包装用的程序集和动态程序集<br/>
		/// </summary>
		public virtual IList<Assembly> GetLoadedAssemblies() {
			return LoadedAssemblies.Where(a => !a.IsDynamic).ToList();
		}

		/// <summary>
		/// Load assembly by name<br/>
		/// 根据名称加载程序集<br/>
		/// </summary>
		public abstract Assembly Load(string name);
		/// <summary>
		/// Load assembly by name object<br/>
		/// 根据名称对象加载程序集<br/>
		/// </summary>
		public abstract Assembly Load(AssemblyName assemblyName);
		/// <summary>
		/// Load assembly from it's binary contents<br/>
		/// 根据二进制内容加载程序集<br/>
		/// </summary>
		public abstract Assembly Load(byte[] rawAssembly);
		/// <summary>
		/// Load assembly from file path<br/>
		/// 根据文件路径加载程序集<br/>
		/// </summary>
		public abstract Assembly LoadFile(string path);
	}
}
