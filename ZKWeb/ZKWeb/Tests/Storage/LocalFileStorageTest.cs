using System;
using System.Linq;
using System.Text;
using ZKWeb.Storage;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Storage {
	[Tests]
	class LocalFileStorageTest {
		public void GetTemplateFile() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WritePluginFile("PluginA", "templates/__test_1.html", "test 1 in plugin a");
				layout.WritePluginFile("PluginB", "templates/__test_1.html", "test 1 in plugin b");
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetTemplateFile("__test_1.html");
				Assert.IsTrue(fileEntry.Exists);
				Assert.IsTrue(!string.IsNullOrEmpty(fileEntry.Filename));
				Assert.IsTrue(!string.IsNullOrEmpty(fileEntry.UniqueIdentifier));
				if (PlatformUtils.RunningOnWindows()) {
					// on unix there no file creation time
					Assert.IsTrue(fileEntry.CreationTimeUtc != DateTime.MinValue);
				}
				Assert.IsTrue(fileEntry.LastAccessTimeUtc != DateTime.MinValue);
				Assert.IsTrue(fileEntry.LastWriteTimeUtc != DateTime.MinValue);
				Assert.Equals(fileEntry.ReadAllText(), "test 1 in plugin b");
				Assert.IsTrue(fileEntry.ReadAllBytes()
					.SequenceEqual(Encoding.UTF8.GetBytes("test 1 in plugin b")));
				Assert.Throws<NotSupportedException>(() => fileEntry.WriteAllText("test readonly"));
				fileEntry = fileStorage.GetTemplateFile("__test_2.html");
				Assert.IsTrue(!fileEntry.Exists);
			}
		}

		public void GetResourceFile() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WritePluginFile("PluginA", "static/__test_1.txt", "test 1 in plugin a");
				layout.WritePluginFile("PluginB", "static/__test_1.txt", "test 1 in plugin b");
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetResourceFile("static", "__test_1.txt");
				Assert.IsTrue(fileEntry.Exists);
				Assert.Equals(fileEntry.ReadAllText(), "test 1 in plugin b");
				Assert.Throws<NotSupportedException>(() => fileEntry.WriteAllText("test readonly"));
				fileEntry = fileStorage.GetResourceFile("static", "__test_2.txt");
				Assert.IsTrue(!fileEntry.Exists);
			}
		}

		public void GetStorageFile() {
			using (var layout = new TestDirectoryLayout()) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetStorageFile("static", "__test_file.txt");
				fileEntry.WriteAllText("test storage file");

				fileEntry = fileStorage.GetStorageFile("static", "__test_file.txt");
				Assert.Equals(fileEntry.ReadAllText(), "test storage file");
				fileEntry.Delete();

				fileEntry = fileStorage.GetStorageFile("static", "__test_file.txt");
				Assert.IsTrue(!fileEntry.Exists);
			}
		}

		public void GetStorageDirectory() {
			using (var layout = new TestDirectoryLayout()) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				fileStorage.GetStorageFile("static", "__test_storage", "__1.txt").WriteAllText("1.txt");
				fileStorage.GetStorageFile("static", "__test_storage", "__2.txt").WriteAllText("2.txt");
				fileStorage.GetStorageFile("static", "__test_storage", "__3.txt").WriteAllText("3.txt");
				fileStorage.GetStorageFile("static", "__test_storage", "child", "child.txt").WriteAllText("child.txt");

				var directoryEntry = fileStorage.GetStorageDirectory("static", "__test_storage");
				var childFiles = directoryEntry.EnumerateFiles();
				Assert.IsTrueWith(childFiles.Any(f => f.Filename == "__1.txt"), childFiles);
				Assert.IsTrueWith(childFiles.Any(f => f.Filename == "__2.txt"), childFiles);
				Assert.IsTrueWith(childFiles.Any(f => f.Filename == "__3.txt"), childFiles);
				Assert.IsTrueWith(childFiles.Any(f => f.ReadAllText() == "1.txt"), childFiles);
				Assert.IsTrueWith(childFiles.Any(f => f.ReadAllText() == "2.txt"), childFiles);
				Assert.IsTrueWith(childFiles.Any(f => f.ReadAllText() == "3.txt"), childFiles);

				var childDirectories = directoryEntry.EnumerateDirectories();
				Assert.IsTrueWith(childDirectories.Any(d => d.DirectoryName == "child"), childDirectories);
				Assert.IsTrueWith(childDirectories
					.First(d => d.DirectoryName == "child")
					.EnumerateFiles().Any(f => f.ReadAllText() == "child.txt"), childDirectories);

				directoryEntry.Delete();
				Assert.IsTrue(!directoryEntry.EnumerateFiles().Any());
				Assert.IsTrue(!directoryEntry.EnumerateDirectories().Any());
			}
		}

		public void ReadAllText_WriteAllText() {
			using (var layout = new TestDirectoryLayout()) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetStorageFile("static", "__test_text.txt");
				fileEntry.WriteAllText("test write text");

				fileEntry = fileStorage.GetStorageFile("static", "__test_text.txt");
				Assert.Equals(fileEntry.ReadAllText(), "test write text");
				fileEntry.Delete();
			}
		}

		public void AppendAllText() {
			using (var layout = new TestDirectoryLayout()) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetStorageFile("static", "__test_text.txt");
				fileEntry.WriteAllText("test write text");
				fileEntry.AppendAllText(" and test append");

				fileEntry = fileStorage.GetStorageFile("static", "__test_text.txt");
				Assert.Equals(fileEntry.ReadAllText(), "test write text and test append");
				fileEntry.Delete();
			}
		}

		public void ReadAllBytes_WriteAllBytes() {
			using (var layout = new TestDirectoryLayout()) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetStorageFile("static", "__test_bytes.txt");
				fileEntry.WriteAllBytes(new byte[] { 0x99, 0x0, 0x88, 0x0a, 0x0a });

				fileEntry = fileStorage.GetStorageFile("static", "__test_bytes.txt");
				var readBytes = fileEntry.ReadAllBytes();
				Assert.IsTrueWith(readBytes.SequenceEqual(new byte[] { 0x99, 0x0, 0x88, 0x0a, 0x0a }), readBytes);
				fileEntry.Delete();
			}
		}
	}
}
