using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests {
	[UnitTest]
	public class MyTest {
		public void TestA() {
			Assert.IsTrue(true);
			Assert.Equals(1, 1);
			Assert.Throws<Exception>(() => { });
		}

		public void TestB() {
			Assert.Skiped("why");
		}
	}
}
