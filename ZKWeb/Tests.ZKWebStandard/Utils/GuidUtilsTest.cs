using System;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class GuidUtilsTest {
		public void SequentialGuid() {
			var a = GuidUtils.SequentialGuid(new DateTime(2016, 8, 8));
			var b = GuidUtils.SequentialGuid(new DateTime(2016, 8, 9));
			Assert.IsTrue(a != Guid.Empty);
			Assert.IsTrue(b != Guid.Empty);
			Assert.IsTrueWith(string.Compare(a.ToString(), b.ToString()) < 0, new { a, b });
		}

		public void SecureSequentialGuid() {
			var a = GuidUtils.SecureSequentialGuid(new DateTime(2016, 8, 8));
			var b = GuidUtils.SecureSequentialGuid(new DateTime(2016, 8, 9));
			Assert.IsTrue(a != Guid.Empty);
			Assert.IsTrue(b != Guid.Empty);
			Assert.IsTrueWith(string.Compare(a.ToString(), b.ToString()) < 0, new { a, b });
		}
	}
}
