using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class ObjectExtensionsTest {
		public void EqualsSupportsNull() {
			Assert.Equals(((string)null).EqualsSupportsNull(""), false);
			Assert.Equals("".EqualsSupportsNull(null), false);
			Assert.Equals(((string)null).EqualsSupportsNull(null), true);
			Assert.Equals("abc".EqualsSupportsNull("abc"), true);
		}

		enum TestEnum {
			Zero = 0,
			One = 1
		}

		public void ConvertOrDefaultT() {
			Assert.Equals("1".ConvertOrDefault<int>(), 1);
			Assert.Equals((1).ConvertOrDefault<int>(), 1);
			Assert.Equals("abc".ConvertOrDefault<int?>(), null);
			Assert.Equals("abc".ConvertOrDefault<int?>(100), 100);
			Assert.Equals("1.0".ConvertOrDefault<decimal?>(), 1.0M);
			Assert.Equals("1".ConvertOrDefault<TestEnum?>(), TestEnum.One);
			Assert.Equals((1).ConvertOrDefault<TestEnum?>(), TestEnum.One);
			Assert.Equals("One".ConvertOrDefault<TestEnum>(), TestEnum.One);
			Assert.Equals("One".ConvertOrDefault<TestEnum?>(), TestEnum.One);
			Assert.Equals(TestEnum.One.ConvertOrDefault<int>(), 1);
			Assert.Equals(TestEnum.One.ConvertOrDefault<string>(), "One");
			Assert.Equals(new List<int>().ConvertOrDefault<int?>(), null);
			Assert.Equals((100).ConvertOrDefault<string>(), "100");
			Assert.Equals("test".ConvertOrDefault<string>(), "test");
			var lst = "[1]".ConvertOrDefault<List<int>>();
			Assert.IsTrueWith(lst.SequenceEqual(new[] { 1 }), lst);
		}

		public void ConvertOrDefault() {
			Assert.Equals("1".ConvertOrDefault(typeof(int), 0), 1);
			Assert.Equals((1).ConvertOrDefault(typeof(int), 0), 1);
			Assert.Equals("abc".ConvertOrDefault(typeof(int?), null), null);
			Assert.Equals("abc".ConvertOrDefault(typeof(int?), 100), 100);
			Assert.Equals("1.0".ConvertOrDefault(typeof(decimal?), null), 1.0M);
			Assert.Equals("1".ConvertOrDefault(typeof(TestEnum?), null), TestEnum.One);
			Assert.Equals((1).ConvertOrDefault(typeof(TestEnum?), null), TestEnum.One);
			Assert.Equals("One".ConvertOrDefault(typeof(TestEnum), null), TestEnum.One);
			Assert.Equals("One".ConvertOrDefault(typeof(TestEnum?), null), TestEnum.One);
			Assert.Equals(TestEnum.One.ConvertOrDefault(typeof(int), null), 1);
			Assert.Equals(TestEnum.One.ConvertOrDefault(typeof(string), null), "One");
			Assert.Equals(new List<int>().ConvertOrDefault(typeof(int?), null), null);
			Assert.Equals((100).ConvertOrDefault(typeof(string), null), "100");
			Assert.Equals("test".ConvertOrDefault(typeof(string), null), "test");
			var lst = "[1]".ConvertOrDefault(typeof(List<int>), null) as List<int>;
			Assert.IsTrueWith(lst.SequenceEqual(new[] { 1 }), lst);
		}

		class TestData {
			public long A { get; set; }
			public string B { get; set; }
			public bool C;
		}

		public void CloneByJson() {
			var data = new TestData() { A = 100, B = "TestString", C = true };
			var dataClone = data.CloneByJson();
			Assert.IsTrue(!object.ReferenceEquals(data, dataClone));
			Assert.Equals(dataClone.A, 100);
			Assert.Equals(dataClone.B, "TestString");
			Assert.Equals(dataClone.C, true);
			data.A = 101;
			Assert.Equals(dataClone.A, 100);
		}
	}
}
