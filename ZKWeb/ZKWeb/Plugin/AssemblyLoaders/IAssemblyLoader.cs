using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Interface for assembly loader
	/// </summary>
	public interface IAssemblyLoader {
		/// <summary>
		/// Get loaded assemblies
		/// It should exclude wrapper assemblies and dynamic assemblies
		/// </summary>
		/// <returns></returns>
		IList<Assembly> GetLoadedAssemblies();

		/// <summary>
		/// Load assembly by name
		/// </summary>
		/// <param name="name">Assembly name, in short or full</param>
		/// <returns></returns>
		Assembly Load(string name);

		/// <summary>
		/// Load assembly by name object
		/// </summary>
		/// <param name="assemblyName">Assembly name object</param>
		/// <returns></returns>
		Assembly Load(AssemblyName assemblyName);

		/// <summary>
		/// Load assembly from it's binary contents
		/// </summary>
		/// <param name="rawAssembly">Assembly binary contents</param>
		/// <returns></returns>
		Assembly Load(byte[] rawAssembly);

		/// <summary>
		/// Load assembly from file path
		/// </summary>
		/// <param name="path">Assembly file path</param>
		/// <returns></returns>
		Assembly LoadFile(string path);
	}
}
