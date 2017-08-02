using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
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

		public void SingleOrDefaultNotThrow() {
			var a = new string[] { };
			var b = new[] { "a" };
			var c = new[] { "a", "b", "c" };
			Assert.Equals(a.SingleOrDefaultNotThrow(), null);
			Assert.Equals(b.SingleOrDefaultNotThrow(), "a");
			Assert.Equals(c.SingleOrDefaultNotThrow(), null);
		}
	}
}
