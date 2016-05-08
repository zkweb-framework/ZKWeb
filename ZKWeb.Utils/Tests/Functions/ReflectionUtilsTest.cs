using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Functions {
	[UnitTest]
	class ReflectionUtilsTest {
		class TestData {
			int a;
			public int GetA() { return a; }
			public void SetA(int value) { a = value; }
		}

		public void MakeSetter() {
			var setter = ReflectionUtils.MakeSetter<TestData, int>("a");
			var data = new TestData();
			setter(data, 1);
			Assert.Equals(data.GetA(), 1);
		}

		public void MakeGetter() {
			var getter = ReflectionUtils.MakeGetter<TestData, int>("a");
			var data = new TestData();
			data.SetA(1);
			Assert.Equals(getter(data), 1);
		}
	}
}
