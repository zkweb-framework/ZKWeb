using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// 支持泛型的Ioc容器的解决器接口
	/// </summary>
	public interface IGenericResolver {
		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个实例
		/// 没有注册或注册了多个时按无法解决时的策略处理
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="ifUnresolved">无法解决时的策略</param>
		/// <param name="serviceKey">关联键</param>
		/// <returns></returns>
		TService Resolve<TService>(IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个或多个实例
		/// 没有注册时返回空列表
		/// </summary>
		/// <typeparam name="TService">服务类型</typeparam>
		/// <param name="serviceKey">关联键</param>
		/// <returns></returns>
		IEnumerable<TService> ResolveMany<TService>(object serviceKey = null);
	}
}
