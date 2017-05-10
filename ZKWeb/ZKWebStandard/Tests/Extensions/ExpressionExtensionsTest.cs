using System;
using System.ComponentModel;
using System.Linq.Expressions;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
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

		public void ReplaceNode() {
			Expression<Func<TestClass, bool>> exprA = a => a.TestMember.Contains("abc");
			Expression<Func<TestClass, bool>> exprB = a => a.TestMember.Contains("asd");
			var exprMerged = Expression.Lambda<Func<TestClass, bool>>(
				Expression.AndAlso(
					exprA.Body,
					exprB.Body.ReplaceNode(exprB.Parameters[0], exprA.Parameters[0])),
				exprA.Parameters[0]);
			var method = exprMerged.Compile();
			Assert.IsTrue(method(new TestClass() { TestMember = "abcasd" }));
			Assert.IsTrue(!method(new TestClass() { TestMember = "abcasp" }));
		}

		public class TestClass {
			[Description("TestMember_Description")]
			public string TestMember { get; set; }
		}
	}
}
