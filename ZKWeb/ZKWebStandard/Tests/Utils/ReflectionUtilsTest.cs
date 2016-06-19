using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Functions {
	[Tests]
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
