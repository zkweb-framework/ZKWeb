using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class ISetExtensionsTest {
		public void AddRange() {
			var set = new SortedSet<int>();
			set.AddRange(new[] { 1, 2, 3 });
			Assert.IsTrueWith(set.SequenceEqual(new[] { 1, 2, 3 }), set);
		}
	}
}
