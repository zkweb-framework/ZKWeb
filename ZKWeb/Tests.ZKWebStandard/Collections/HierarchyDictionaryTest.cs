using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Collections;
using ZKWebStandard.Testing;

namespace Tests.ZKWebStandard.Collections {
	[Tests]
	class HierarchyDictionaryTest : ScenarioBase {
		public void Get() {
			HierarchyDictionary<int, string> dict = null;
			string value1 = null;
			string value2 = null;
			string value3 = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("get values by key", () => {
				value1 = dict[new[] { 1, 2, 3 }];
				value2 = dict[new[] { 1, 8, 8 }];
				value3 = dict[new[] { 1, 8, 8, 0, 1 }];
			});

			Then("values are available", () => {
				Assert.Equals(value1, "123");
				Assert.Equals(value2, "188");
				Assert.Equals(value3, "18801");
			});
		}

		public void GetError() {
			var dict = new HierarchyDictionary<int, string>();
			Assert.Throws<KeyNotFoundException>(() => {
#pragma warning disable S1481 // Unused local variables should be removed
				var _ = dict[new[] { 1, 0, 0 }];
#pragma warning restore S1481 // Unused local variables should be removed
			});
		}

		public void Keys() {
			HierarchyDictionary<int, string> dict = null;
			ICollection<IEnumerable<int>> keys = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
			});

			When("get all keys", () => {
				keys = dict.Keys;
			});

			Then("keys are available", () => {
				Assert.Equals(keys.Count, 2);
				Assert.IsTrue(keys.Any(k => k.SequenceEqual(new[] { 1, 2, 3 })));
				Assert.IsTrue(keys.Any(k => k.SequenceEqual(new[] { 1, 8, 8 })));
			});
		}

		public void Values() {
			HierarchyDictionary<int, string> dict = null;
			ICollection<string> values = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
			});

			When("get all values", () => {
				values = dict.Values;
			});

			Then("values are available", () => {
				Assert.Equals(values.Count, 2);
				Assert.IsTrue(values.Contains("123"));
				Assert.IsTrue(values.Contains("188"));
			});
		}

		public void Count() {
			HierarchyDictionary<int, string> dict = null;
			int count = 0;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("get count", () => {
				count = dict.Count;
			});

			Then("count is correct", () => {
				Assert.Equals(dict.Count, 3);
			});
		}

		public void Clear() {
			HierarchyDictionary<int, string> dict = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("clear dictionary", () => {
				dict.Clear();
			});

			Then("dictionary is cleared", () => {
				Assert.Equals(dict.Count, 0);
			});
		}

		public void ContainsKey() {
			HierarchyDictionary<int, string> dict = null;
			bool result1 = false;
			bool result2 = false;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
			});

			When("check contains key", () => {
				result1 = dict.ContainsKey(new[] { 1, 2, 3 });
				result2 = dict.ContainsKey(new[] { 1, 2, 3, 0, 0 });
			});

			Then("check results are correct", () => {
				Assert.IsTrue(result1);
				Assert.IsFalse(result2);
			});
		}

		public void GetEnumerator() {
			HierarchyDictionary<int, string> dict = null;
			List<KeyValuePair<IEnumerable<int>, string>> list = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("get list of key value pairs", () => {
				list = dict.ToList();
			});

			Then("list is available", () => {
				Assert.Equals(list.Count, 3);
				Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 2, 3 })));
				Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 8, 8 })));
				Assert.IsTrue(list.Any(pair => pair.Key.SequenceEqual(new[] { 1, 8, 8, 0, 1 })));
				Assert.IsTrue(list.Any(pair => pair.Value == "123"));
				Assert.IsTrue(list.Any(pair => pair.Value == "188"));
				Assert.IsTrue(list.Any(pair => pair.Value == "18801"));
			});
		}

		public void Remove() {
			HierarchyDictionary<int, string> dict = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("remove values by key", () => {
				dict.Remove(new[] { 1, 8, 8 });
				dict.Remove(new[] { 1, 2, 3 });
			});

			Then("values removed", () => {
				Assert.IsTrue(!dict.ContainsKey(new[] { 1, 2, 3 }));
				Assert.IsTrue(!dict.ContainsKey(new[] { 1, 8, 8 }));
				Assert.IsTrue(dict.ContainsKey(new[] { 1, 8, 8, 0, 1 }));
			});
		}

		public void TryGetValue() {
			HierarchyDictionary<int, string> dict = null;
			bool result1 = false;
			bool result2 = false;
			string value1 = null;
			string value2 = null;

			Given("a hierarchy dictionary", () => {
				dict = new HierarchyDictionary<int, string>();
				dict[new[] { 1, 2, 3 }] = "123";
				dict[new[] { 1, 8, 8 }] = "188";
				dict[new[] { 1, 8, 8, 0, 1 }] = "18801";
			});

			When("try get values by key", () => {
				result1 = dict.TryGetValue(new[] { 1, 2, 3 }, out value1);
				result2 = dict.TryGetValue(new[] { 1, 2, 3, 0, 0 }, out value2);
			});

			Then("values available if key exists", () => {
				Assert.IsTrue(result1);
				Assert.Equals(value1, "123");
				Assert.IsFalse(result2);
				Assert.Equals(value2, null);
			});
		}
	}
}