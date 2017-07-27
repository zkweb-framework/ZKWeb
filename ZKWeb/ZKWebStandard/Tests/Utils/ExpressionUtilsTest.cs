using System.Collections.Generic;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class ExpressionUtilsTest {
		public void MakeMemberEqualiventExpression() {
			var expression = ExpressionUtils.MakeMemberEqualiventExpression<ICollection<int>>("Count", 3);
			var method = expression.Compile();
			Assert.Equals(method(new[] { 1, 2, 3 }), true);
		}
	}
}
