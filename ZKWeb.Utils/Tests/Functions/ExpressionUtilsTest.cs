using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Functions {
	[UnitTest]
	class ExpressionUtilsTest {
		public void MakeMemberEqualiventExpression() {
			var expression = ExpressionUtils.MakeMemberEqualiventExpression<ICollection<int>>("Count", 3);
			var method = expression.Compile();
			Assert.Equals(method(new[] { 1, 2, 3 }), true);
		}
	}
}
