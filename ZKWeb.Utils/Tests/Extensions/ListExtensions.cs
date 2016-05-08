using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class ListExtensions {
		public void AddBefore() {
			var list = new List<int>() { 1, 2, 4 };
			list.AddBefore(x => x == 4, 3);
			Assert.IsTrueWith(list.SequenceEqual(new[] { 1, 2, 3, 4 }), list);
		}

		public void AddAfter() {
			var list = new List<int>() { 1, 2, 4 };
			list.AddAfter(x => x == 2, 3);
			Assert.IsTrueWith(list.SequenceEqual(new[] { 1, 2, 3, 4 }), list);
		}
	}
}
