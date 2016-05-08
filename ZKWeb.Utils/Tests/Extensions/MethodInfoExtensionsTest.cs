using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class MethodInfoExtensionsTest {
		public void GetFullName() {
			var methodInfo = this.GetType().GetMethod("GetFullName");
			var fullname = methodInfo.GetFullName();
			Assert.Equals(fullname, "ZKWeb.Utils.Tests.Extensions.MethodInfoExtensionsTest.GetFullName");
		}
	}
}
