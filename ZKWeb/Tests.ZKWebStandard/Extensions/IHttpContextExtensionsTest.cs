using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWebStandard.Tests.Extensions
{
    [Tests]
#pragma warning disable S101 // Types should be named in camel case
    class IHttpContextExtensionsTest
    {
#pragma warning restore S101 // Types should be named in camel case
        public void PutData()
        {
            var list = new[] { "a", "b", "c" };
            HttpManager.CurrentContext.PutData("TestPutData", list);
            Assert.Equals(HttpManager.CurrentContext.GetData<string[]>("TestPutData"), list);
        }

        public void GetData()
        {
            var list = new string[] { "a", "b", "c" };
            HttpManager.CurrentContext.PutData("TestGetData", list);
            Assert.Equals(HttpManager.CurrentContext.GetData<string[]>("TestGetData"), list);
        }

        public void GetOrCreateData()
        {
            HttpManager.CurrentContext.PutData("TestGetData", "abc");
            Assert.Equals(HttpManager.CurrentContext.GetOrCreateData("TestGetData", () => "def"), "abc");
            Assert.Equals(HttpManager.CurrentContext.GetOrCreateData("TestCreateData", () => "def"), "def");
        }

        public void RemoveData()
        {
            HttpManager.CurrentContext.PutData("TestRemoveData", "abc");
            HttpManager.CurrentContext.RemoveData("TestRemoveData");
            Assert.Equals(HttpManager.CurrentContext.GetData<string>("TestRemoveData"), null);
        }

        public void GetCookie()
        {
            HttpManager.CurrentContext.PutCookie("TestGetCookie", "abc");
            Assert.Equals(HttpManager.CurrentContext.GetCookie("TestGetCookie"), "abc");
        }

        public void PutCookie()
        {
            HttpManager.CurrentContext.PutCookie("TestPutCookie", "abc");
            Assert.Equals(HttpManager.CurrentContext.GetCookie("TestPutCookie"), "abc");
        }

        public void RemoveCookie()
        {
            HttpManager.CurrentContext.PutCookie("TestRemoveCookie", "abc");
            HttpManager.CurrentContext.RemoveCookie("TestRemoveCookie");
            Assert.Equals(HttpManager.CurrentContext.GetCookie("TestRemoveCookie"), "");
        }

        public void GetClientDevice()
        {
            Assert.Equals(HttpManager.CurrentContext.GetClientDevice(), DeviceTypes.Desktop);
            using (HttpManager.OverrideContext("", "GET"))
            {
                var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
                request.headers["User-Agent"] = "Mozilla/5.0 (Linux; U; Android 2.3; en-us) AppleWebKit/999+";
                Assert.Equals(HttpManager.CurrentContext.GetClientDevice(), DeviceTypes.Mobile);
            }
            using (HttpManager.OverrideContext("", "GET"))
            {
                var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
                request.headers["User-Agent"] = "Mozilla/5.0 (Linux) AppleWebKit/999+";
                Assert.Equals(HttpManager.CurrentContext.GetClientDevice(), DeviceTypes.Desktop);
            }
        }

        public void SetClientDeviceToCookies()
        {
            using (HttpManager.OverrideContext("", "GET"))
            {
                HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Desktop);
                Assert.Equals(HttpManager.CurrentContext.GetClientDevice(), DeviceTypes.Desktop);
            }
            using (HttpManager.OverrideContext("", "GET"))
            {
                HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Mobile);
                Assert.Equals(HttpManager.CurrentContext.GetClientDevice(), DeviceTypes.Mobile);
            }
        }
    }
}
