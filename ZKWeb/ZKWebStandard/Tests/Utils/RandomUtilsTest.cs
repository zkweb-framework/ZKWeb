using System.Linq;
using System.Text.RegularExpressions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class RandomUtilsTest {
		public void SystemRandomBytes() {
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.SystemRandomBytes(20)).ToList();
			Assert.IsTrueWith(array.All(x => x.Length == 20), array);
			Assert.IsTrueWith(array.Any(x => !x.SequenceEqual(array.First())), array);
		}

		public void SystemRandomInt() {
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.SystemRandomInt()).ToList();
			Assert.IsTrueWith(array.Any(x => x != array.First()), array);
		}

		public void RandomInt() {
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.RandomInt(0, 100)).ToList();
			Assert.IsTrueWith(array.All(x => x >= 0 && x <= 100), array);
			Assert.IsTrueWith(array.Any(x => x != array.First()), array);
		}

		public void RandomSelection() {
			var options = new[] { 1, 2, 3, 4, 5 };
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.RandomSelection(options)).ToList();
			Assert.IsTrueWith(array.All(x => x >= 1 && x <= 5), array);
			Assert.IsTrueWith(array.Any(x => x != array.First()), array);
		}

		enum EmptyEnum { }

		enum TestEnum { A = 1, B = 2, C = 3 }

		public void RandomEnum() {
			Assert.Equals(RandomUtils.RandomEnum<EmptyEnum>(), (EmptyEnum)0);
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.RandomEnum<TestEnum>()).ToList();
			Assert.IsTrueWith(array.All(x => x >= TestEnum.A && x <= TestEnum.C), array);
			Assert.IsTrueWith(array.Any(x => x != array.First()), array);
		}

		public void RandomString() {
			var array = Enumerable.Range(0, 20).Select(_ => RandomUtils.RandomString(20)).ToList();
			Assert.IsTrueWith(array.All(x => Regex.IsMatch(x, @"^[\w\d]{20}$")), array);
			Assert.IsTrueWith(array.Any(b => b != array.First()), array);
		}
	}
}
