using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest {
	/// <summary>
	/// 标记类是单元测试类
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class UnitTestAttribute : Attribute {
	}
}
