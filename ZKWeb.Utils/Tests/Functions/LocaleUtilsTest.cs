using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Functions {
	[UnitTest]
	class LocaleUtilsTest {
		public void SetThreadLanguage() {
			Assert.IsTrue(LocaleUtils.SetThreadLanguage("zh-CN"));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "zh-CN");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "zh-CN");
			Assert.IsTrue(LocaleUtils.SetThreadLanguage("en-US"));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "en-US");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "en-US");
			Assert.IsTrue(!LocaleUtils.SetThreadLanguage(null));
			Assert.IsTrue(!LocaleUtils.SetThreadLanguage("NotExist"));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "en-US");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "en-US");
		}

		public void SetThreadTimezone() {
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("China Standard Time"));
			var time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local));
			Assert.IsTrue(LocaleUtils.SetThreadTimezone("GMT Standard Time"));
			localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local));
			Assert.IsTrue(!LocaleUtils.SetThreadTimezone(null));
			Assert.IsTrue(!LocaleUtils.SetThreadTimezone("NotExist"));
			localTime = time.ToClientTime();
			Assert.Equals(localTime, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local));
		}

		public void SetThreadLanguageAutomatic() {
			// 无cookies, 无浏览器语言，传入默认语言
			HttpContextUtils.RemoveCookie(LocaleUtils.LanguageKey);
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, "zh-CN"));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "zh-CN");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "zh-CN");
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, "en-US"));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "en-US");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "en-US");
			// 无cookies, 有浏览器语言但不启用，不传入默认语言
			using (HttpContextUtils.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpContextUtils.CurrentContext.Request;
				request.userLanguages = new [] { "NotExist", "zh-CN" };
				Assert.IsTrue(!LocaleUtils.SetThreadLanguageAutomatic(false, null));
				Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "en-US");
				Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "en-US");
			}
			// 无cookies, 有浏览器语言且启用，不传入默认语言
			using (HttpContextUtils.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpContextUtils.CurrentContext.Request;
				request.userLanguages = new[] { "NotExist", "zh-CN" };
				Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(true, null));
				Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "zh-CN");
				Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "zh-CN");
			}
			// 有cookies，无浏览器语言，不传入默认语言
			HttpContextUtils.PutCookie(LocaleUtils.LanguageKey, "en-US");
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, null));
			Assert.Equals(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, "en-US");
			Assert.Equals(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag, "en-US");
		}

		public void SetThreadTimezoneAutomatic() {
			// 无cookies, 传入默认时区
			HttpContextUtils.RemoveCookie(LocaleUtils.TimeZoneKey);
			LocaleUtils.SetThreadTimezoneAutomatic("China Standard Time");
			var timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone.StandardName, "China Standard Time");
			LocaleUtils.SetThreadTimezoneAutomatic("GMT Standard Time");
			timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone.StandardName, "GMT Standard Time");
			// 无cookies, 不传入默认时区
			Assert.IsTrue(!LocaleUtils.SetThreadTimezoneAutomatic(null));
			timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone.StandardName, "GMT Standard Time");
			// 有cookies, 不传入默认时区
			HttpContextUtils.PutCookie(LocaleUtils.TimeZoneKey, "China Standard Time");
			Assert.IsTrue(LocaleUtils.SetThreadTimezoneAutomatic(null));
			timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone.StandardName, "China Standard Time");
		}
	}
}
