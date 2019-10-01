using System.FastReflection;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions
{
    [Tests]
    class MethodInfoExtensionsTest
    {
        public void GetFullName()
        {
            var methodInfo = this.GetType().FastGetMethod("GetFullName");
            var fullname = methodInfo.GetFullName();
            Assert.Equals(fullname, "ZKWebStandard.Tests.Extensions.MethodInfoExtensionsTest.GetFullName");
        }
    }
}
