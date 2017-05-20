using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Interface of assembly loader<br/>
	/// 程序加载器的接口<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var assemblyLoader = Application.Ioc.Resolve&lt;IAssemblyLoader&gt;();
	/// var assembly = assemblyLoader.Load("System.DrawingCore");
	/// </code>
	/// </example>
	public interface IAssemblyLoader {
		/// <summary>
		/// Get loaded assemblies<br/>
		/// It should exclude wrapper assemblies and dynamic assemblies<br/>
		/// 获取已加载的程序集<br/>
		/// 它应该排除包装用的程序集和动态程序集<br/>
		/// </summary>
		/// <returns></returns>
		IList<Assembly> GetLoadedAssemblies();

		/// <summary>
		/// Load assembly by name<br/>
		/// 根据名称加载程序集<br/>
		/// </summary>
		/// <param name="name">Assembly name, in short or full</param>
		/// <returns></returns>
		Assembly Load(string name);

		/// <summary>
		/// Load assembly by name object<br/>
		/// 根据名称对象加载程序集<br/>
		/// </summary>
		/// <param name="assemblyName">Assembly name object</param>
		/// <returns></returns>
		Assembly Load(AssemblyName assemblyName);

		/// <summary>
		/// Load assembly from it's binary contents<br/>
		/// 根据二进制内容加载程序集<br/>
		/// </summary>
		/// <param name="rawAssembly">Assembly binary contents</param>
		/// <returns></returns>
		Assembly Load(byte[] rawAssembly);

		/// <summary>
		/// Load assembly from file path<br/>
		/// 根据文件路径加载程序集<br/>
		/// </summary>
		/// <param name="path">Assembly file path</param>
		/// <returns></returns>
		Assembly LoadFile(string path);
	}
}
