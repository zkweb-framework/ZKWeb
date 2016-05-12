using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Collections;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// RWLock的扩展函数
	/// </summary>
	public static class ReaderWriterLockSlimExtensions {
		/// <summary>
		/// 获取读取锁
		/// 此函数必须和using一起使用，否则可能发生不可预见的后果
		/// </summary>
		/// <param name="rwlock">锁对象</param>
		/// <returns></returns>
		public static IDisposable WithReadLock(this ReaderWriterLockSlim rwlock) {
			rwlock.EnterReadLock();
			return new SimpleDisposable(() => rwlock.ExitReadLock());
		}

		/// <summary>
		/// 获取写入锁
		/// 此函数必须和using一起使用，否则可能发生不可预见的后果
		/// </summary>
		/// <param name="rwlock">锁对象</param>
		/// <returns></returns>
		public static IDisposable WithWriteLock(this ReaderWriterLockSlim rwlock) {
			rwlock.EnterWriteLock();
			return new SimpleDisposable(() => rwlock.ExitWriteLock());
		}
	}
}
