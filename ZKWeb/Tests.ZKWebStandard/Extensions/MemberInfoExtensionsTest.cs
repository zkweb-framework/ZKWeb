using System;
using System.ComponentModel;
using System.FastReflection;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class MemberInfoExtensionsTest {
		class TestData {
			[Description("TestDescription")]
			public string TestProperty { get; set; }
		}

		abstract class Base {
			[Description("TestBaseDescription")]
			public abstract void TestMethod();
		}

		class Derived : Base {
			public override void TestMethod() { }
		}

		public void GetAttribute() {
			var info = typeof(TestData).FastGetProperty("TestProperty");
			var attribute = info.GetAttribute<DescriptionAttribute>();
			Assert.IsTrue(attribute != null);
			Assert.Equals(attribute.Description, "TestDescription");
			var notExistAttribute = info.GetAttribute<TestsAttribute>();
			Assert.Equals(notExistAttribute, null);
		}

		public void GetAttributes() {
			var info = typeof(TestData).FastGetProperty("TestProperty");
			var attributes = info.GetAttributes<Attribute>();
			Assert.Equals(attributes.OfType<DescriptionAttribute>().First().Description, "TestDescription");
		}

		public void GetInheritedAttributes() {
			var info = typeof(Derived).FastGetMethod("TestMethod");
			var attributes = info.GetAttributes<Attribute>(true);
			Assert.Equals(attributes.OfType<DescriptionAttribute>().First().Description, "TestBaseDescription");
		}
	}
}
