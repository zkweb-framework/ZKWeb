using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class MemberInfoExtensionsTest {
		class TestData {
			[Description("TestDescription")]
			public string TestProperty { get; set; }
		}

		public void GetAttribute() {
			var info = typeof(TestData).GetProperty("TestProperty");
			var attribute = info.GetAttribute<DescriptionAttribute>();
			Assert.IsTrue(attribute != null);
			Assert.Equals(attribute.Description, "TestDescription");
			var notExistAttribute = info.GetAttribute<UnitTestAttribute>();
			Assert.Equals(notExistAttribute, null);
		}

		public void GetAttributes() {
			var info = typeof(TestData).GetProperty("TestProperty");
			var attributes = info.GetAttributes<Attribute>();
			Assert.Equals(attributes.OfType<DescriptionAttribute>().First().Description, "TestDescription");
		}
	}
}
