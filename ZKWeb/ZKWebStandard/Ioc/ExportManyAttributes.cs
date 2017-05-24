using System;
using System.Linq;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Attribute for register type to IoC container with itself and it's base type and interfaces<br/>
	/// 用于根据类型的基类和接口，注册类型到IoC容器的属性<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	/// <seealso cref="IContainer"/>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = false)]
	public class ExportManyAttribute : ExportAttributeBase {
		/// <summary>
		/// Service key<br/>
		/// 服务键<br/>
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// Except types<br/>
		/// 排除的类型列表<br/>
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// Also register with non public service types<br/>
		/// 同时注册到非公开的服务类型<br/>
		/// </summary>
		public bool NonPublic { get; set; }
		/// <summary>
		/// Unregister service types before register<br/>
		/// Please sure it won't unintentionally remove innocent implementations<br/>
		/// 注销已有的服务类型的实现<br/>
		/// 请确保它不会意外的移除无辜的实现<br/>
		/// </summary>
		public bool ClearExists { get; set; }

		/// <summary>
		/// Register implementation type to container<br/>
		/// 注册实现类型到容器<br/>
		/// </summary>
		public override void RegisterToContainer(IContainer container, Type type, ReuseType reuseType) {
			var serviceTypes = Container.GetImplementedServiceTypes(type, NonPublic);
			// Apply except types
			if (Except != null && Except.Any()) {
				serviceTypes = serviceTypes.Where(t => !Except.Contains(t));
			}
			var serviceTypesArray = serviceTypes.ToList();
			// Apply clear exist
			if (ClearExists) {
				foreach (var serviceType in serviceTypesArray) {
					container.Unregister(serviceType, ContractKey);
				}
			}
			// Register to container
			container.RegisterMany(serviceTypesArray, type, reuseType, ContractKey);
		}
	}
}
