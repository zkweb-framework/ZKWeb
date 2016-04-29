using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class ByteArrayExtensionsTest {
		public void ToHex() {
			var bytes = new byte[] { 1, 0x12, 0x13 };
			Assert.Equals(bytes.ToHex(), "011213");
		}
	}
}
