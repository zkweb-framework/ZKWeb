using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Cache {
	[UnitTest]
	class CacheIsolationPoliciesTest {
		public void ByDevice() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Device");
			using (HttpContextUtils.OverrideContext("", "GET")) {
				HttpDeviceUtils.SetClientDeviceToCookies(HttpDeviceUtils.DeviceTypes.Desktop);
				Assert.Equals(policy.GetIsolationKey(), HttpDeviceUtils.DeviceTypes.Desktop);
			}
			using (HttpContextUtils.OverrideContext("", "GET")) {
				HttpDeviceUtils.SetClientDeviceToCookies(HttpDeviceUtils.DeviceTypes.Mobile);
				Assert.Equals(policy.GetIsolationKey(), HttpDeviceUtils.DeviceTypes.Mobile);
			}
		}

		public void ByLocale() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Locale");
			LocaleUtils.SetThreadLanguage("zh-CN");
			LocaleUtils.SetThreadTimezoneAutomatic("China Standard Time");
			Assert.Equals(policy.GetIsolationKey(), Pair.Create(
				"zh-CN", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")));
			LocaleUtils.SetThreadLanguage("en-US");
			LocaleUtils.SetThreadTimezoneAutomatic("GMT Standard Time");
			Assert.Equals(policy.GetIsolationKey(), Pair.Create(
				"en-US", TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")));
		}

		public void ByUrl() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Url");
			using (HttpContextUtils.OverrideContext("/test?a=1", "GET")) {
				Assert.Equals(policy.GetIsolationKey(), "/test?a=1");
			}
			using (HttpContextUtils.OverrideContext("/test?a=2", "GET")) {
				Assert.Equals(policy.GetIsolationKey(), "/test?a=2");
			}
		}
	}
}
