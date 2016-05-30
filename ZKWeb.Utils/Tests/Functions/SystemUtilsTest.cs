using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Functions {
	[UnitTest]
	class SystemUtilsTest {
		public void GetUsedMemoryBytes() {
			var value = SystemUtils.GetUsedMemoryBytes();
			Assert.IsTrueWith(value > 0, value);
		}
	}
}
