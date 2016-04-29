using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class ExpressionExtensionsTest {
		public void GetMemberInfo() {
			Expression<Func<TestClass, string>> expr = t => t.TestMember;
			var memberInfo = expr.GetMemberInfo();
			Assert.Equals(memberInfo.Name, "TestMember");
			Assert.Throws<ArgumentException>(() => {
				expr = t => "a";
				memberInfo = expr.GetMemberInfo();
			});
		}

		public void GetMemberAttribute() {
			Expression<Func<TestClass, string>> expr = t => t.TestMember;
			var attr = expr.GetMemberAttribute<DescriptionAttribute>();
			Assert.Equals(attr.Description, "TestMember_Description");
		}

		public class TestClass {
			[Description("TestMember_Description")]
			public string TestMember { get; set; }
		}
	}
}
