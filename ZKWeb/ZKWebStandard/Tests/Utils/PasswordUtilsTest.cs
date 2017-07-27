using System;
using System.Text;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class PasswordUtilsTest {
		public void PBKDF2Sum() {
			var hash = PasswordUtils.PBKDF2Sum(
				Encoding.UTF8.GetBytes("123456"),
				Encoding.UTF8.GetBytes("12344321"));
			Assert.Equals(hash.ToHex(),
				"47e00677444b6d16c36d347a4fea584792fb4a50fe93e762c7e1adf4f73e2475");
		}

		public void Md5Sum() {
			var hash = PasswordUtils.Md5Sum(Encoding.UTF8.GetBytes("123456"));
			Assert.Equals(hash.ToHex(), "e10adc3949ba59abbe56e057f20f883e");
		}

		public void Sha1Sum() {
			var hash = PasswordUtils.Sha1Sum(Encoding.UTF8.GetBytes("123456"));
			Assert.Equals(hash.ToHex(), "7c4a8d09ca3762af61e59520943dc26494f8941b");
		}

		public void PasswordInfoTest() {
			var info = PasswordInfo.FromPassword("123456");
			Assert.IsTrue(!info.Check("12345"));
			Assert.IsTrue(!info.Check(null));
			Assert.IsTrue(!info.Check(""));
			Assert.IsTrue(info.Check("123456"));
			Assert.Throws<ArgumentNullException>(() => PasswordInfo.FromPassword(null));
			Assert.Throws<ArgumentNullException>(() => PasswordInfo.FromPassword(""));
		}
	}
}
