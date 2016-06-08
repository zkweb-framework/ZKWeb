using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Localize;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Serialize {
	[UnitTest]
	class JsonNetTest {
		public void All() {
			// 测试序列化T类型
			var translated = new T("__OriginalString");
			Assert.Equals(JsonConvert.SerializeObject(translated),
				JsonConvert.SerializeObject(translated.ToString()));
			// 测试防止反序列化时使用原来的对象
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
