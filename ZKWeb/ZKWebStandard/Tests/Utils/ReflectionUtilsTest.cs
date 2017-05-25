using ZKWebStandard.Utils;
using ZKWebStandard.Testing;
using System.Collections.Generic;

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

		public void GetGenericArguments() {
			var args = ReflectionUtils.GetGenericArguments(typeof(Dictionary<string, int>), typeof(IDictionary<,>));
			Assert.Equals(args.Length, 2);
			Assert.Equals(args[0], typeof(string));
			Assert.Equals(args[1], typeof(int));
		}
	}
}
