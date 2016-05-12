using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class ReaderWriterLockSlimExtensionsTest {
		public void WithReadLock() {
			var rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (rwlock.WithReadLock()) {
				Assert.Throws<LockRecursionException>(() => rwlock.TryEnterReadLock(1));
			}
			Assert.IsTrue(rwlock.TryEnterReadLock(1));
			rwlock.ExitReadLock();
		}

		public void WithWriteLock() {
			var rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (rwlock.WithWriteLock()) {
				Assert.Throws<LockRecursionException>(() => rwlock.TryEnterWriteLock(1));
			}
			Assert.IsTrue(rwlock.TryEnterWriteLock(1));
			rwlock.ExitWriteLock();
		}
	}
}
