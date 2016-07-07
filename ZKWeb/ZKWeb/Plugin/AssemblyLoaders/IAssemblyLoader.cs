using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// 程序集载入器的接口
	/// </summary>
	public interface IAssemblyLoader {
		/// <summary>
		/// 获取当前已载入的程序集列表
		/// </summary>
		/// <returns></returns>
		IList<Assembly> GetLoadedAssemblies();

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		/// <param name="name">程序集名称</param>
		/// <returns></returns>
		Assembly Load(string name);

		/// <summary>
		/// 从二进制数据载入程序集
		/// </summary>
		/// <param name="rawAssembly">程序集的二进制数据内容</param>
		/// <returns></returns>
		Assembly Load(byte[] rawAssembly);

		/// <summary>
		/// 从文件载入程序集
		/// </summary>
		/// <param name="path">程序集的文件路径</param>
		/// <returns></returns>
		Assembly LoadFile(string path);
	}
}
