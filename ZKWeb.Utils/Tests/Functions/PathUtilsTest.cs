using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Functions {
	[UnitTest]
	class PathUtilsTest {
		public void WebRoot() {
			var webroot = PathUtils.WebRoot.Value;
			Assert.IsTrueWith(File.Exists(Path.Combine(webroot, "Web.config")), webroot);
		}

		public void SecureCombine() {
			Assert.Equals(PathUtils.SecureCombine("a", "b", "c"), Path.Combine("a", "b", "c"));
			Assert.Throws<ArgumentException>(() => PathUtils.SecureCombine("a", "/b", "c"));
			Assert.Throws<ArgumentException>(() => PathUtils.SecureCombine("a", "\\b", "c"));
			Assert.Throws<ArgumentException>(() => PathUtils.SecureCombine("a", "", "c"));
			Assert.Throws<ArgumentException>(() => PathUtils.SecureCombine("a", "..", "c"));
			Assert.Throws<ArgumentException>(() => PathUtils.SecureCombine("a/../b", "c"));
		}
	}
}
