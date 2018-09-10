using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
#pragma warning disable S101 // Types should be named in camel case
	class ISetExtensionsTest {
#pragma warning restore S101 // Types should be named in camel case
		public void AddRange() {
			var set = new SortedSet<int>();
			Assert.Equals(set.AddRange(new[] { 1, 2, 3 }), 3);
			Assert.IsTrueWith(set.SequenceEqual(new[] { 1, 2, 3 }), set);
			Assert.Equals(set.AddRange(new[] { 3, 4, 5 }), 2);
			Assert.IsTrueWith(set.SequenceEqual(new[] { 1, 2, 3, 4, 5 }), set);
		}
	}
}
