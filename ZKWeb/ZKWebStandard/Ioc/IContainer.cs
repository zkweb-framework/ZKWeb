using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for IoC container<br/>
	/// IoC容器的接口<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	/// <example>
	/// <code language="cs">
	/// void Example() {
	/// 	var animals = Application.Ioc.ResolveMany&lt;IAnimal&gt;()
	/// 	// animals contains instances of Dog and Cow
	///
	/// 	var animalManager = Application.Ioc.Resolve&lt;IAnimalManager&gt;();
	/// 	// animalManager is AnimalManager
	/// 	
	/// 	var otherAnimalManager = Application.Ioc.Resolve&lt;IAnimalManager&gt;();
	/// 	// animalManager only create once, otherAnimalManager == animalManager
	/// }
	///
	/// public interface IAnimal { }
	///
	/// [ExportMany]
	/// public class Dog : IAnimal { }
	///
	/// [ExportMany]
	/// public class Cow : IAnimal { }
	///
	/// public interface IAnimalManager { }
	///
	/// [ExportMany, SingletonUsage]
	/// public class AnimalManager : IAnimalManager {
	/// 	// inject animals
	/// 	public AnimalManager(IEnumerable&lt;IAnimal&gt; animals) { }
	/// }
	///	/// </code>
	///	/// </example>
	public interface IContainer :
		IRegistrator,
		IGenericRegistrator,
		IResolver,
		IGenericResolver,
		IScopeDisposer,
		IDisposable {
		/// <summary>
		/// Clone the container<br/>
		/// 克隆容器<br/>
		/// </summary>
		/// <returns></returns>
		object Clone();
	}
}
