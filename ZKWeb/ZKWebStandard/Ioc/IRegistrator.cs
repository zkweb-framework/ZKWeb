using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Ioc容器的注册器接口
	/// </summary>
	public interface IRegistrator {
		/// <summary>
		/// 注册实现类型到服务类型，并关联指定的键
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="implementationType">实现类型</param>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void Register(Type serviceType, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册实现类型到多个服务类型，并关联指定的键
		/// </summary>
		/// <param name="serviceTypes">服务类型列表</param>
		/// <param name="implementationType">实现类型</param>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterMany(IList<Type> serviceTypes, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册实现类型到它自身和继承的类型和实现的接口，并关联指定的键
		/// 部分系统类型和接口会被忽略
		/// </summary>
		/// <param name="implementationType">实现类型</param>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		/// <param name="nonPublicServiceTypes">是否包含私有类型</param>
		void RegisterMany(Type implementationType,
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false);

		/// <summary>
		/// 注册实例到服务类型，并关联指定的键
		/// 默认是单例模式
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="instance">实例</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterInstance(Type serviceType, object instance, object serviceKey = null);

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="factor">生成函数</param>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterDelegate(Type serviceType, Func<object> factor,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册使用属性导出的类型
		/// 应支持以下属性
		/// - ExportAttribute (System.ComponentModel.Composition)
		/// - ExportManyAttribute
		/// </summary>
		/// <param name="types"></param>
		void RegisterExports(IEnumerable<Type> types);

		/// <summary>
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="serviceKey">关联键</param>
		void Unregister(Type serviceType, object serviceKey = null);

		/// <summary>
		/// 取消所有注册的生成函数
		/// </summary>
		void UnregisterAll();
	}
}
