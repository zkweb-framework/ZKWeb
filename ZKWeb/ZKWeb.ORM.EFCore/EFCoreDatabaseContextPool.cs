using System;
using System.Collections.Concurrent;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Database context pool<br/>
	/// 数据库上下文的缓存池<br/>
	/// </summary>
	public class EFCoreDatabaseContextPool : IDisposable {
		/// <summary>
		/// Cached contexts<br/>
		/// 缓存的上下文列表<br/>
		/// </summary>
		protected ConcurrentQueue<EFCoreDatabaseContext> Contexts { get; set; }
		/// <summary>
		/// Context factory<br/>
		/// 上下文的工厂函数<br/>
		/// </summary>
		protected Func<EFCoreDatabaseContext> Factory { get; set; }
		/// <summary>
		/// Cache limit<br/>
		/// 缓存限制<br/>
		/// </summary>
		protected int Limit { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public EFCoreDatabaseContextPool(Func<EFCoreDatabaseContext> factory, int? limit = null) {
			Contexts = new ConcurrentQueue<EFCoreDatabaseContext>();
			Factory = factory;
			Limit = limit ?? 100;
		}

		/// <summary>
		/// Get a database context<br/>
		/// 获取一个数据库上下文<br/>
		/// </summary>
		public EFCoreDatabaseContext Get() {
			if (Contexts.TryDequeue(out var context)) {
				return context;
			}
			context = Factory();
			context.Pool = this;
			return context;
		}

		/// <summary>
		/// Return database context to pool<br/>
		/// 返回数据库上下文给池<br/>
		/// </summary>
		public bool Return(EFCoreDatabaseContext context) {
			if (Contexts.Count < Limit) {
				Contexts.Enqueue(context);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Dispose all contexts in pool<br/>
		/// 释放池中的所有上下文<br/>
		/// </summary>
		public void Dispose() {
			var contexts = Contexts;
			Contexts = new ConcurrentQueue<EFCoreDatabaseContext>();
			foreach (var context in contexts) {
				context.Dispose();
			}
		}
	}
}
