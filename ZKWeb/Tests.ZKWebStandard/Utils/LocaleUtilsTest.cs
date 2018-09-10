using System;
using System.Threading;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;
using System.Globalization;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class LocaleUtilsTest {
		public void SetThreadLanguage() {
			Assert.IsTrue(LocaleUtils.SetThreadLanguage("zh-CN"));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "zh-CN");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "zh-CN");
			Assert.IsTrue(LocaleUtils.SetThreadLanguage("en-US"));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "en-US");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "en-US");
			Assert.IsTrue(!LocaleUtils.SetThreadLanguage(null));
			Assert.IsTrue(!LocaleUtils.SetThreadLanguage("NotExist"));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "en-US");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "en-US");
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
			var context = HttpManager.CurrentContext;
			// No cookies, no accept languages, with default language
			context.RemoveCookie(LocaleUtils.LanguageKey);
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, "zh-CN"));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "zh-CN");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "zh-CN");
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, "en-US"));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "en-US");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "en-US");
			// No cookies, have accept languages but not using, no default language
			using (HttpManager.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.headers["Accept-Language"] = "NotExist,zh-CN;q=0.7";
				Assert.IsTrue(!LocaleUtils.SetThreadLanguageAutomatic(false, null));
				Assert.Equals(CultureInfo.CurrentCulture.Name, "en-US");
				Assert.Equals(CultureInfo.CurrentUICulture.Name, "en-US");
			}
			// No cookies, have accept languages and it can be used, no default language
			using (HttpManager.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.headers["Accept-Language"] = "NotExist,zh-CN;q=0.7";
				Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(true, null));
				Assert.Equals(CultureInfo.CurrentCulture.Name, "zh-CN");
				Assert.Equals(CultureInfo.CurrentUICulture.Name, "zh-CN");
			}
			// Have cookies, no accept languages, no default language
			context.PutCookie(LocaleUtils.LanguageKey, "en-US");
			Assert.IsTrue(LocaleUtils.SetThreadLanguageAutomatic(false, null));
			Assert.Equals(CultureInfo.CurrentCulture.Name, "en-US");
			Assert.Equals(CultureInfo.CurrentUICulture.Name, "en-US");
		}

		public void SetThreadTimezoneAutomatic() {
			var context = HttpManager.CurrentContext;
			// No cookies, with default timezone
			context.RemoveCookie(LocaleUtils.TimeZoneKey);
			LocaleUtils.SetThreadTimezoneAutomatic("China Standard Time");
			var timezone = context.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone, LocaleUtils.GetTimezoneInfo("China Standard Time"));
			LocaleUtils.SetThreadTimezoneAutomatic("GMT Standard Time");
			timezone = context.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone, LocaleUtils.GetTimezoneInfo("GMT Standard Time"));
			// No cookies, no default timezone
			Assert.IsTrue(!LocaleUtils.SetThreadTimezoneAutomatic(null));
			timezone = context.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone, LocaleUtils.GetTimezoneInfo("GMT Standard Time"));
			// Have cookies, no default timezone
			context.PutCookie(LocaleUtils.TimeZoneKey, "China Standard Time", new HttpCookieOptions());
			Assert.IsTrue(LocaleUtils.SetThreadTimezoneAutomatic(null));
			timezone = context.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			Assert.Equals(timezone, LocaleUtils.GetTimezoneInfo("China Standard Time"));
		}
	}
}
