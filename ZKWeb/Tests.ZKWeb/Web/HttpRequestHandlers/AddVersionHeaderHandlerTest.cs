using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.HttpRequestHandlers
{
    [Tests]
    class AddVersionHeaderHandlerTest
    {
        public void OnRequest()
        {
            var handler = new AddVersionHeaderHandler();
            using (HttpManager.OverrideContext("", "GET"))
            {
                var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
                handler.OnRequest();
                Assert.Equals(response.headers["X-ZKWeb-Version"][0], Application.FullVersion);
            }
        }
    }
}
