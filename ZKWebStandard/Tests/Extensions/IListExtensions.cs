using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class IListExtensions {
		public void FindIndex() {
			IList<int> list = new List<int>() { 1, 2, 4 };
			Assert.Equals(list.FindIndex(x => x % 2 == 0), 1);
			Assert.Equals(list.FindIndex(x => x % 4 == 0), 2);
			Assert.Equals(list.FindIndex(x => x % 8 == 0), -1);
			Assert.Equals(list.FindIndex(2, x => x % 2 == 0), 2);
			Assert.Equals(list.FindIndex(-1, x => x % 2 == 0), 1);
		}

		public void FindLastIndex() {
			IList<int> list = new List<int>() { 1, 2, 4 };
			Assert.Equals(list.FindLastIndex(x => x % 2 == 0), 2);
			Assert.Equals(list.FindLastIndex(x => x % 4 == 0), 2);
			Assert.Equals(list.FindLastIndex(x => x % 8 == 0), -1);
			Assert.Equals(list.FindLastIndex(1, x => x % 2 == 0), 1);
			Assert.Equals(list.FindLastIndex(10000, x => x % 2 == 0), 2);
		}

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
