using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class IEnumerableExtensionsTest {
		public void ConcatIfNotNull() {
			var a = new[] { "a", "b", "c" };
			var b = a.ConcatIfNotNull("d");
			Assert.Equals(b.Count(), 4);
			Assert.IsTrueWith(b.Contains("d"), b);
			var c = a.ConcatIfNotNull(null);
			Assert.Equals(c.Count(), 3);
			Assert.IsTrueWith(!c.Contains(null), c);
		}

		public void ForEach() {
			var a = new[] { "a", "b", "c" };
			var b = new List<string>();
			a.ForEach(c => b.Add(c));
			Assert.Equals(b.Count, 3);
			Assert.IsTrueWith(a.SequenceEqual(b), new { a, b });
		}
	}
}
