using ZKWebStandard.Utils;
using ZKWebStandard.Testing;
using System.Collections.Generic;
using System.FastReflection;
using System;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class ReflectionUtilsTest {
		class TestData {
			int a;
			public int GetA() { return a; }
			public void SetA(int value) { a = value; }
			public string Generic<T>() { return typeof(T).Name; }
			public string MultiGeneric<A, B>() { return typeof(A).Name + typeof(B).Name; }
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

		public void MakeInvoker() {
			var getA = typeof(TestData).FastGetMethod(nameof(TestData.GetA));
			var setA = typeof(TestData).FastGetMethod(nameof(TestData.SetA));
			var getAInvoker = ReflectionUtils.MakeInvoker(getA);
			var setAInvoker = ReflectionUtils.MakeInvoker(setA);
			var data = new TestData();
			setAInvoker(data, new object[] { 123 });
			Assert.Equals(getAInvoker(data, null), 123);

			var generic = typeof(TestData).FastGetMethod(nameof(TestData.Generic));
			var multiGeneric = typeof(TestData).FastGetMethod(nameof(TestData.MultiGeneric));
			var genericInvoker = ReflectionUtils.MakeInvoker(generic, typeof(TestData));
			var multiGenericInvoker = ReflectionUtils.MakeInvoker(multiGeneric, new Type[] { typeof(TestData), typeof(Assert) });
			Assert.Equals(genericInvoker(data, null), "TestData");
			Assert.Equals(multiGenericInvoker(data, null), "TestDataAssert");
		}

		public void GetGenericArguments() {
			var args = ReflectionUtils.GetGenericArguments(typeof(Dictionary<string, int>), typeof(IDictionary<,>));
			Assert.Equals(args.Length, 2);
			Assert.Equals(args[0], typeof(string));
			Assert.Equals(args[1], typeof(int));
		}
	}
}
