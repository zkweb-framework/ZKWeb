using System;
using System.IO;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
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

		public void EnsureParentDirectory() {
			var tempPath = Path.GetTempPath();
			var dirPath = Path.Combine(tempPath, "EnsureParentDirectoryTest");
			var filePath = Path.Combine(dirPath, "1.txt");
			if (Directory.Exists(dirPath)) {
				Directory.Delete(dirPath, true);
			}
			PathUtils.EnsureParentDirectory(filePath);
			Assert.IsTrue(Directory.Exists(dirPath));
			Directory.Delete(dirPath, true);
		}
	}
}
