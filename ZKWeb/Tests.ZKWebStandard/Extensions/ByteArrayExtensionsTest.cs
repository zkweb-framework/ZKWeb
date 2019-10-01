using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions
{
    [Tests]
    class ByteArrayExtensionsTest
    {
        public void ToHex()
        {
            var bytes = new byte[] { 1, 0x12, 0x13 };
            Assert.Equals(bytes.ToHex(), "011213");
        }
    }
}
