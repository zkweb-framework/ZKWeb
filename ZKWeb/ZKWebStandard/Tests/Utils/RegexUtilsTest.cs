using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class RegexUtilsTest {
		public void Email() {
			Assert.IsTrue(RegexUtils.Validators.Email.IsMatch("a@b.c"));
			Assert.IsTrue(RegexUtils.Validators.Email.IsMatch("a@b.c.x"));
			Assert.IsTrue(!RegexUtils.Validators.Email.IsMatch("a@bc"));
			Assert.IsTrue(!RegexUtils.Validators.Email.IsMatch("ab.c"));
		}

		public void ChinaMobile() {
			Assert.IsTrue(RegexUtils.Validators.ChinaMobile.IsMatch("13788888888"));
			Assert.IsTrue(!RegexUtils.Validators.ChinaMobile.IsMatch("12788888888"));
			Assert.IsTrue(!RegexUtils.Validators.ChinaMobile.IsMatch("13788888888a"));
		}

		public void Digits() {
			Assert.IsTrue(RegexUtils.Validators.Digits.IsMatch("12345"));
			Assert.IsTrue(RegexUtils.Validators.Digits.IsMatch("-12345"));
			Assert.IsTrue(!RegexUtils.Validators.Digits.IsMatch("12345.0"));
			Assert.IsTrue(!RegexUtils.Validators.Digits.IsMatch("12345abc"));
		}

		public void Decimal() {
			Assert.IsTrue(RegexUtils.Validators.Decimal.IsMatch("12345"));
			Assert.IsTrue(RegexUtils.Validators.Decimal.IsMatch("-12345"));
			Assert.IsTrue(RegexUtils.Validators.Decimal.IsMatch("12345.0"));
			Assert.IsTrue(!RegexUtils.Validators.Decimal.IsMatch("12345."));
			Assert.IsTrue(!RegexUtils.Validators.Decimal.IsMatch("12345abc"));
		}

		public void Tel() {
			Assert.IsTrue(RegexUtils.Validators.Tel.IsMatch("+86 0769-00000000"));
			Assert.IsTrue(!RegexUtils.Validators.Tel.IsMatch("0769-00000000abc"));
		}
	}
}
