using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Collections;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class HierarchyDictionaryTest {
		public void GetSet() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			Assert.Equals(dict[new[] { 1, 2, 3 }], "123");
			Assert.Equals(dict[new[] { 1, 8, 8 }], "188");
			Assert.Equals(dict[new[] { 1, 8, 8, 0, 1 }], "18801");
			Assert.Throws<KeyNotFoundException>(() => {
				var _ = dict[new[] { 1, 0, 0 }];
			});
		}

		public void KeysValues() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			var keys = dict.Keys;
			var values = dict.Values;
			Assert.Equals(keys.Count, 2);
			Assert.Equals(values.Count, 2);
			Assert.IsTrue(keys.Any(k => k.SequenceEqual(new[] { 1, 2, 3 })));
			Assert.IsTrue(keys.Any(k => k.SequenceEqual(new[] { 1, 8, 8 })));
			Assert.IsTrue(values.Contains("123"));
			Assert.IsTrue(values.Contains("188"));
		}

		public void Count() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			Assert.Equals(dict.Count, 3);
		}

		public void Clear() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			Assert.Equals(dict.Count, 3);
			dict.Clear();
			Assert.Equals(dict.Count, 0);
		}

		public void ContainsKey() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 2, 3 }));
			Assert.IsTrue(!dict.ContainsKey(new[] { 1, 2, 3, 0, 0 }));
		}

		public void GetEnumerator() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			var list = dict.ToList();
			Assert.Equals(list.Count, 3);
			Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 2, 3 })));
			Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 8, 8 })));
			Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 8, 8, 0, 1 })));
			Assert.IsTrue(list.Any(pair => pair.Value == "123"));
			Assert.IsTrue(list.Any(pair => pair.Value == "188"));
			Assert.IsTrue(list.Any(pair => pair.Value == "18801"));
		}

		public void Remove() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 2, 3 }));
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 8, 8 }));
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 8, 8, 0, 1 }));
			dict.Remove(new[] { 1, 8, 8 });
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 2, 3 }));
			Assert.IsTrue(!dict.ContainsKey(new[] { 1, 8, 8 }));
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 8, 8, 0, 1 }));
			dict.Remove(new[] { 1, 2, 3 });
			Assert.IsTrue(!dict.ContainsKey(new[] { 1, 2, 3 }));
			Assert.IsTrue(!dict.ContainsKey(new[] { 1, 8, 8 }));
			Assert.IsTrue(dict.ContainsKey(new[] { 1, 8, 8, 0, 1 }));
		}

		public void TryGetValue() {
			var dict = new HierarchyDictionary<int, string>();
			dict[new[] { 1, 2, 3 }] = "123";
			dict[new[] { 1, 8, 8 }] = "188";
			dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			Assert.IsTrue(dict.TryGetValue(new[] { 1, 2, 3 }, out var value));
			Assert.Equals(value, "123");
			Assert.IsTrue(!dict.TryGetValue(new[] { 1, 0, 0 }, out value));
			Assert.Equals(value, null);
		}
	}
}
