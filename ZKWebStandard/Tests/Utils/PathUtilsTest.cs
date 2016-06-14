using System;
using System.IO;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Functions {
	[Tests]
	class PathUtilsTest {
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
