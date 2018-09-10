using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using ZKWeb.Localize;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Serialize {
	[Tests]
	class JsonNetTest {
		public void All() {
			// Serialize T
			var translated = new T("__OriginalString");
			Assert.Equals(JsonConvert.SerializeObject(translated),
				JsonConvert.SerializeObject(translated.ToString()));
			// Avoid add items to exist collection
			var data = JsonConvert.DeserializeObject<TestData>("{ Array: [ 4, 5, 6 ] }");
			Assert.IsTrueWith(data.Array.SequenceEqual(new[] { 4, 5, 6 }), data);
		}

		class TestData {
			public IList<int> Array { get; set; }
			public TestData() {
				Array = new List<int>() { 1, 2, 3 };
			}
		}
	}
}
