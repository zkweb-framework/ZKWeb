using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Reflection;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// IContainer => IServiceProvider Adapter<br/>
	/// 转换IContainer到IServiceProvider的接口<br/>
	/// </summary>
	public class ServiceProviderAdapter : IServiceProvider {
		/// <summary>
		/// Container<br/>
		/// 容器<br/>
		/// </summary>
		protected IContainer Container { get; set; }
		/// <summary>
		/// MethodInfo of ListFactory<br/>
		/// ListFactory的MethodInfo<br/>
		/// </summary>
		protected MethodInfo ListFactoryMethod { get; set; }
		/// <summary>
		/// MethodInfo of IEnumerableFactory<br/>
		/// IEnumerableFactory的MethodInfo<br/>
		/// </summary>
		protected MethodInfo IEnumerableFactoryMethod { get; set; }
		/// <summary>
		/// MethodInfo of LazyFactory<br/>
		/// LazyFactory的MethodInfo<br/>
		/// </summary>
		protected MethodInfo LazyFactoryMethod { get; set; }

		/// <summary>
		/// Initliaze<br/>
		/// 初始化<br/>
		/// </summary>
		public ServiceProviderAdapter(IContainer container) {
			Container = container;
			ListFactoryMethod = GetType().FastGetMethod(nameof(ListFactory));
			IEnumerableFactoryMethod = GetType().FastGetMethod(nameof(IEnumerableFactory));
			LazyFactoryMethod = GetType().FastGetMethod(nameof(LazyFactory));
		}

		/// <summary>
		/// Resolve List&lt;T&gt;<br/>
		/// 解决List&lt;T&gt;<br/>
		/// </summary>
		public List<T> ListFactory<T>() {
			return Container.ResolveMany<T>().ToList();
		}

		/// <summary>
		/// Resolve IEnumerable&lt;T&gt;<br/>
		/// 解决List&lt;T&gt;<br/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> IEnumerableFactory<T>() {
			return Container.ResolveMany<T>();
		}

		/// <summary>
		/// Resolve Lazy&lt;T&gt;<br/>
		/// 解决Lazy&lt;T&gt;<br/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolve"></param>
		/// <returns></returns>
		public Lazy<T> LazyFactory<T>(Func<object> resolve) {
			return new Lazy<T>(() => (T)resolve());
		}

		/// <summary>
		/// Resolve service<br/>
		/// 解决服务<br/>
		/// </summary>
		public object GetService(Type serviceType) {
			var isFunc = false;
			var isLazy = false;
			var isIList = false;
			var isIEnumerable = false;
			// Detect Func<T>
			Type funcReturnType = null;
			if (serviceType.IsGenericType &&
				serviceType.GetGenericTypeDefinition() == typeof(Func<>)) {
				isFunc = true;
				serviceType = funcReturnType = serviceType.GetGenericArguments()[0];
			}
			// Detect Lazy<T>
			if (serviceType.IsGenericType &&
				serviceType.GetGenericTypeDefinition() == typeof(Lazy<>)) {
				isLazy = true;
				serviceType = serviceType.GetGenericArguments()[0];
			}
			// Detect IList<T> and IEnumerable<T>
			if (!serviceType.IsGenericType) {
			} else if (serviceType.GetGenericTypeDefinition() == typeof(List<>) ||
				serviceType.GetGenericTypeDefinition() == typeof(IList<>) ||
				serviceType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
				isIList = true;
				serviceType = serviceType.GetGenericArguments()[0];
			} else if (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
				isIEnumerable = true;
				serviceType = serviceType.GetGenericArguments()[0];
			}
			// Resolve implementation
			var resolve = new Func<object>(() => {
				if (isIList) {
					return ReflectionUtils.MakeInvoker(ListFactoryMethod, serviceType)(this, null);
				} else if (isIEnumerable) {
					return ReflectionUtils.MakeInvoker(IEnumerableFactoryMethod, serviceType)(this, null);
				}
				return Container.Resolve(serviceType, IfUnresolved.ReturnDefault);
			});
			if (isLazy) {
				var originalResolve = resolve;
				var invoker = ReflectionUtils.MakeInvoker(LazyFactoryMethod, serviceType);
				resolve = () => invoker(this, new object[] { originalResolve });
			}
			if (isFunc) {
				var originalResolve = resolve;
				var invoker = ReflectionUtils.MakeInvoker(
					ContainerFactoryBuilder.ToGenericFactoryMethod, funcReturnType);
				resolve = () => invoker(null, new object[] { originalResolve });
			}
			return resolve();
		}
	}
}
