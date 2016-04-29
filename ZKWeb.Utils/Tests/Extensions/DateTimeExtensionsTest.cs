using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class DateTimeExtensionsTest {
		public void ToClientTime() {
			HttpContextUtils.RemoveData(LocaleUtils.TimeZoneKey);
			var time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Assert.Equals(time.ToClientTime(), time.ToLocalTime());
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("China Standard Time"));
			var localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local));
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("GMT Standard Time"));
			localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local));
		}

		public void ToClientTimeString() {
			HttpContextUtils.RemoveData(LocaleUtils.TimeZoneKey);
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("China Standard Time"));
			var time = new DateTime(2000, 1, 2, 13, 14, 50, DateTimeKind.Utc);
			var timeString = time.ToClientTimeString();
			Assert.Equals(timeString, "2000/01/02 21:14:50");
		}

		public void FromClientTime() {
			HttpContextUtils.RemoveData(LocaleUtils.TimeZoneKey);
			var time = new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local);
			Assert.Equals(time.FromClientTime(), time.ToUniversalTime());
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("China Standard Time"));
			var utcTime = time.FromClientTime();
			Assert.Equals(utcTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("GMT Standard Time"));
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
