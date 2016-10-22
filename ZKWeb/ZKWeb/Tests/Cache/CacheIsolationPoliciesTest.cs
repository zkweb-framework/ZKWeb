using System;
using ZKWeb.Cache;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Tests.Cache {
	[Tests]
	class CacheIsolationPoliciesTest {
		public void ByDevice() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Device");
			using (HttpManager.OverrideContext("", "GET")) {
				HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Desktop);
				Assert.Equals(policy.GetIsolationKey(), DeviceTypes.Desktop);
			}
			using (HttpManager.OverrideContext("", "GET")) {
				HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Mobile);
				Assert.Equals(policy.GetIsolationKey(), DeviceTypes.Mobile);
			}
		}

		public void ByLocale() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Locale");
			LocaleUtils.SetThreadLanguage("zh-CN");
			LocaleUtils.SetThreadTimezoneAutomatic("China Standard Time");
			Assert.Equals(policy.GetIsolationKey(), Pair.Create(
				"zh-CN", LocaleUtils.GetTimezoneInfo("China Standard Time")));
			LocaleUtils.SetThreadLanguage("en-US");
			LocaleUtils.SetThreadTimezoneAutomatic("GMT Standard Time");
			Assert.Equals(policy.GetIsolationKey(), Pair.Create(
				"en-US", LocaleUtils.GetTimezoneInfo("GMT Standard Time")));
		}

		public void ByUrl() {
			var policy = Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Url");
			using (HttpManager.OverrideContext("/test?a=1", "GET")) {
				Assert.Equals(policy.GetIsolationKey(), "/test?a=1");
			}
			using (HttpManager.OverrideContext("/test?a=2", "GET")) {
				Assert.Equals(policy.GetIsolationKey(), "/test?a=2");
			}
		}
	}
}
