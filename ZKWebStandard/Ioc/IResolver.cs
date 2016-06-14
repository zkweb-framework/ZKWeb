using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Ioc容器的解决器接口
	/// </summary>
	public interface IResolver {
		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个实例
		/// 没有注册或注册了多个时按无法解决时的策略处理
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="ifUnresolved">无法解决时的策略</param>
		/// <param name="serviceKey">关联键</param>
		/// <returns></returns>
		object Resolve(Type serviceType, IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个或多个实例
		/// 没有注册时返回空列表
		/// </summary>
		/// <param name="serviceType">服务类型</param>
		/// <param name="serviceKey">关联键</param>
		/// <returns></returns>
		IEnumerable<object> ResolveMany(Type serviceType, object serviceKey = null);
	}
}
