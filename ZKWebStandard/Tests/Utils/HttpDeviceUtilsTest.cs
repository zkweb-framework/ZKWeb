using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWebStandard.Collections;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;
/*
 * TODO FIXME
namespace ZKWebStandard.Tests.Functions {
	[Tests]
	class HttpDeviceUtilsTest {
		public void GetClientDevice() {
			Assert.Equals(HttpDeviceUtils.GetClientDevice(), HttpDeviceUtils.DeviceTypes.Desktop);
			using (HttpManager.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpContextUtils.CurrentContext.Request;
				request.userAgent = "Mozilla/5.0 (Linux; U; Android 2.3; en-us) AppleWebKit/999+";
				Assert.Equals(HttpDeviceUtils.GetClientDevice(), HttpDeviceUtils.DeviceTypes.Mobile);
			}
			using (HttpManager.OverrideContext("", "GET")) {
				var request = (HttpRequestMock)HttpContextUtils.CurrentContext.Request;
				request.userAgent = "Mozilla/5.0 (Linux) AppleWebKit/999+";
				Assert.Equals(HttpDeviceUtils.GetClientDevice(), HttpDeviceUtils.DeviceTypes.Desktop);
			}
		}

		public void SetClientDeviceToCookies() {
			using (HttpManager.OverrideContext("", "GET")) {
				HttpDeviceUtils.SetClientDeviceToCookies(HttpDeviceUtils.DeviceTypes.Desktop);
				Assert.Equals(HttpDeviceUtils.GetClientDevice(), HttpDeviceUtils.DeviceTypes.Desktop);
			}
			using (HttpManager.OverrideContext("", "GET")) {
				HttpDeviceUtils.SetClientDeviceToCookies(HttpDeviceUtils.DeviceTypes.Mobile);
				Assert.Equals(HttpDeviceUtils.GetClientDevice(), HttpDeviceUtils.DeviceTypes.Mobile);
			}
		}
	}
}
*/