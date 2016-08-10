using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// 支持泛型的Ioc容器的注册器接口
	/// </summary>
	public interface IGenericRegistrator {
		/// <summary>
		/// 注册实现类型到服务类型，并关联指定的键
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <typeparam name="TImplementation">实现类型</typeparam>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void Register<TService, TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册实现类型到它自身和继承的类型，并关联指定的键
		/// 部分系统自带的类型会被忽略
		/// </summary>
		/// <typeparam name="TImplementation">实现类型</typeparam>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		/// <param name="nonPublicServiceTypes">是否包含私有类型</param>
		void RegisterMany<TImplementation>(
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false);

		/// <summary>
		/// 注册实例到服务类型，并关联指定的键
		/// 默认是单例模式
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="instance">实例</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterInstance<TService>(TService instance, object serviceKey = null);

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="factory">生成函数</param>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterDelegate<TService>(Func<TService> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="serviceKey">关联键</param>
		void Unregister<TService>(object serviceKey = null);
	}
}
