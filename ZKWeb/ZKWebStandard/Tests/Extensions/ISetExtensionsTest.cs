using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class ISetExtensionsTest {
		public void AddRange() {
			var set = new SortedSet<int>();
			set.AddRange(new[] { 1, 2, 3 });
			Assert.IsTrueWith(set.SequenceEqual(new[] { 1, 2, 3 }), set);
		}
	}
}
