using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// Ioc容器的注册器接口
	/// </summary>
	public interface IRegistrator {
		/// <summary>
		/// 注册TImplementation到TService，并关联键serviceKey
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <typeparam name="TImplementation">实现类型</typeparam>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void Register<TService, TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册TImplementation到它自身，并关联键serviceKey
		/// </summary>
		/// <typeparam name="TImplementation">实现类型</typeparam>
		/// <param name="reuseType">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void Register<TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册TImplementation到它自身和继承的类型，并关联键serviceKey
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
		/// 注册实例instance到TService，并关联键serviceKey
		/// 默认是单例模式
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="instance">实例</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterInstance<TService>(TService instance, object serviceKey = null);

		/// <summary>
		/// 注册生成函数factor到TService，并关联键serviceKey
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="factor">生成函数</param>
		/// <param name="reuse">重用策略</param>
		/// <param name="serviceKey">关联键</param>
		void RegisterDelegate<TService>(
			Func<TService> factor, ReuseType reuse = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// 注册使用属性导出的类型
		/// 应支持以下属性
		/// - ExportAttribute (System.ComponentModel.Composition)
		/// - ExportManyAttribute
		/// </summary>
		/// <param name="types"></param>
		void RegisterExports(IEnumerable<Type> types);

		/// <summary>
		/// 取消注册到TService并关联了serviceKey的生成函数
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="serviceKey">关联键</param>
		void Unregister<TService>(object serviceKey = null);
	}
}
