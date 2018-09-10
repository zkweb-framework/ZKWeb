using System.IO;
using System.Linq;
using ZKWeb.Plugin;
using ZKWeb.Storage;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Tests.Storage {
	[Tests]
	class LocalPathManagerTest {
		public void GetPluginDirectories() {
			var pluginDirectoriesConfig = new[] { "App_Data/__TestPluginsA", "App_Data/__TestPluginsB" };
			using (new TestDirectoryLayout(pluginDirectoriesConfig)) {
				var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
				var pathManager = Application.Ioc.Resolve<LocalPathManager>();
				var pluginDirectories = pathManager.GetPluginDirectories();
				Assert.Equals(pluginDirectories.Count, 2);
				Assert.Equals(pluginDirectories[0],
					Path.GetFullPath(Path.Combine(pathConfig.WebsiteRootDirectory, pluginDirectoriesConfig[0])));
				Assert.Equals(pluginDirectories[1],
					Path.GetFullPath(Path.Combine(pathConfig.WebsiteRootDirectory, pluginDirectoriesConfig[1])));
			}
		}

		public void GetTemplateFullPath() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WritePluginFile("PluginA", "templates/__test_1.html", "test 1 in plugin a");
				layout.WritePluginFile("PluginB", "templates/__test_2.html", "test 2 in plugin b");
				layout.WritePluginFile("PluginB", "templates/__test_3.html", "test 3 in plugin b");
				layout.WritePluginFile("PluginB",
					"templates.mobile/__test_3.html", "test 3 in plugin b for mobile");
				layout.WriteAppDataFile("templates/__test_3.html", "test 3 in appdata");
				var pathManager = Application.Ioc.Resolve<LocalPathManager>();
				var pluginManager = Application.Ioc.Resolve<PluginManager>();
				var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();

				var candidates = pathManager.GetTemplateFullPathCandidates("__test_1.html").ToList();
				Assert.Equals(candidates.Count, 6);
				Assert.Equals(candidates[0], PathUtils.SecureCombine(
					pathConfig.AppDataDirectory, "templates.desktop", "__test_1.html"));
				Assert.Equals(candidates[1], PathUtils.SecureCombine(
					pluginManager.Plugins[1].Directory, "templates.desktop", "__test_1.html"));
				Assert.Equals(candidates[2], PathUtils.SecureCombine(
					pluginManager.Plugins[0].Directory, "templates.desktop", "__test_1.html"));
				Assert.Equals(candidates[3], PathUtils.SecureCombine(
					pathConfig.AppDataDirectory, "templates", "__test_1.html"));
				Assert.Equals(candidates[4], PathUtils.SecureCombine(
					pluginManager.Plugins[1].Directory, "templates", "__test_1.html"));
				Assert.Equals(candidates[5], PathUtils.SecureCombine(
					pluginManager.Plugins[0].Directory, "templates", "__test_1.html"));

				candidates = pathManager.GetTemplateFullPathCandidates("PluginA:__test_1.html").ToList();
				Assert.Equals(candidates.Count, 2);
				Assert.Equals(candidates[0], PathUtils.SecureCombine(
					pluginManager.Plugins[0].Directory, "templates.desktop", "__test_1.html"));
				Assert.Equals(candidates[1], PathUtils.SecureCombine(
					pluginManager.Plugins[0].Directory, "templates", "__test_1.html"));

				Assert.Equals(
					File.ReadAllText(pathManager.GetTemplateFullPath("__test_1.html")),
					"test 1 in plugin a");
				Assert.Equals(
					File.ReadAllText(pathManager.GetTemplateFullPath("__test_2.html")),
					"test 2 in plugin b");
				Assert.Equals(
					File.ReadAllText(pathManager.GetTemplateFullPath("__test_3.html")),
					"test 3 in appdata");
				Assert.Equals(pathManager.GetTemplateFullPath("__test_4.html"), null);

				using (HttpManager.OverrideContext("", "GET")) {
					HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Mobile);
					Assert.Equals(
						File.ReadAllText(pathManager.GetTemplateFullPath("__test_1.html")),
						"test 1 in plugin a");
					Assert.Equals(
						File.ReadAllText(pathManager.GetTemplateFullPath("__test_3.html")),
						"test 3 in plugin b for mobile");
					Assert.Equals(
						File.ReadAllText(pathManager.GetTemplateFullPath("PluginB:__test_2.html")),
						"test 2 in plugin b");
				}
			}
		}

		public void GetResourceFullPath() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WritePluginFile("PluginA", "static/__test_1.txt", "test 1 in plugin a");
				layout.WritePluginFile("PluginB", "static/__test_2.txt", "test 2 in plugin b");
				layout.WritePluginFile("PluginB", "static/__test_3.txt", "test 3 in plugin b");
				layout.WriteAppDataFile("static/__test_3.txt", "test 3 in appdata");
				var pathManager = Application.Ioc.Resolve<LocalPathManager>();
				var pluginManager = Application.Ioc.Resolve<PluginManager>();
				var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();

				var candidates = pathManager.GetResourceFullPathCandidates("static/__test_1.txt").ToList();
				Assert.Equals(candidates.Count, 3);
				Assert.Equals(candidates[0], PathUtils.SecureCombine(
					pathConfig.AppDataDirectory, "static/__test_1.txt"));
				Assert.Equals(candidates[1], PathUtils.SecureCombine(
					pluginManager.Plugins[1].Directory, "static/__test_1.txt"));
				Assert.Equals(candidates[2], PathUtils.SecureCombine(
					pluginManager.Plugins[0].Directory, "static/__test_1.txt"));

				Assert.Equals(
					File.ReadAllText(pathManager.GetResourceFullPath("static/__test_1.txt")),
					"test 1 in plugin a");
				Assert.Equals(
					File.ReadAllText(pathManager.GetResourceFullPath("static/__test_2.txt")),
					"test 2 in plugin b");
				Assert.Equals(
					File.ReadAllText(pathManager.GetResourceFullPath("static/__test_3.txt")),
					"test 3 in appdata");
				Assert.Equals(pathManager.GetResourceFullPath("static/__test_4.txt"), null);
			}
		}

		public void GetStorageFullPath() {
			using (new TestDirectoryLayout()) {
				var pathManager = Application.Ioc.Resolve<LocalPathManager>();
				var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
				Assert.Equals(
					pathManager.GetStorageFullPath("static/__test_1.txt"),
					PathUtils.SecureCombine(pathConfig.AppDataDirectory, "static/__test_1.txt"));
			}
		}
	}
}
