using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Collections {
	[UnitTest]
	class SimpleDisposableTest {
		public void All() {
			// 手动多次执行
			int count = 0;
			var obj = new SimpleDisposable(() => count += 1);
			Assert.Equals(count, 0);
			obj.Dispose();
			Assert.Equals(count, 1);
			obj.Dispose();
			Assert.Equals(count, 1);
			// 使用using
			count = 0;
			using (new SimpleDisposable(() => count += 1)) {
			}
			Assert.Equals(count, 1);
		}
	}
}
