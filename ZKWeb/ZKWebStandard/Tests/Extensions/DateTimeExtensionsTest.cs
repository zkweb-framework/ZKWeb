using System;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class DateTimeExtensionsTest {
		public void ToClientTime() {
			HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
			var time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Assert.Equals(time.ToClientTime(), time.ToLocalTime());
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("Asia/Shanghai"));
			var localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local));
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("Etc/GMT"));
			localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local));
		}

		public void ToClientTimeString() {
			HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("Asia/Shanghai"));
			var time = new DateTime(2000, 1, 2, 13, 14, 50, DateTimeKind.Utc);
			var timeString = time.ToClientTimeString();
			Assert.Equals(timeString, "2000/01/02 21:14:50");
		}

		public void FromClientTime() {
			HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
			var time = new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local);
			Assert.Equals(time.FromClientTime(), time.ToUniversalTime());
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("Asia/Shanghai"));
			var utcTime = time.FromClientTime();
			Assert.Equals(utcTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("Etc/GMT"));
			utcTime = time.FromClientTime();
			Assert.Equals(utcTime, new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Utc));
		}

		public void Truncate() {
			var time = DateTime.UtcNow;
			Assert.Equals(time.Truncate().Millisecond, 0);
		}

		public void ToTimestamp() {
			var time = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc);
			Assert.Equals((int)time.ToTimestamp().TotalSeconds, 86400);
		}
	}
}
