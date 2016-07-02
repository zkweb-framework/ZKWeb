using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using ZKWeb.Toolkits.ProjectCreator.Model;
using ZKWeb.Toolkits.ProjectCreator.Utils;
using System.Linq;

namespace ZKWeb.Toolkits.ProjectCreator {
	/// <summary>
	/// 项目创建器
	/// </summary>
	public class ProjectCreator {
		/// <summary>
		/// 创建项目的参数
		/// </summary>
		public CreateProjectParameters Parameters { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parameters">创建项目的参数</param>
		public ProjectCreator(CreateProjectParameters parameters) {
			Parameters = parameters;
		}

		/// <summary>
		/// 自动检测储存模板的文件夹
		/// </summary>
		/// <returns></returns>
		protected virtual string AutoDetectTemplatesDirectory() {
			var path = Path.GetDirectoryName(typeof(ProjectCreator).GetTypeInfo().Assembly.Location);
			while (!Directory.Exists(Path.Combine(path, "Tools"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Detect templates directory failed");
				}
			}
			return Path.Combine(path, "Tools", "Templates");
		}

		/// <summary>
		/// 写入配置内容
		/// </summary>
		/// <param name="outputPath">config.json的路径</param>
		protected virtual void WriteConfigObject(string outputPath) {
			var pluginDirectories = new List<string>();
			var plugins = new List<string>();
			if (string.IsNullOrEmpty(Parameters.UseDefaultPlugins)) {
				// 不使用默认插件
				pluginDirectories.Add("./");
				plugins.Add(Parameters.ProjectName);
			} else {
				// 使用默认插件
				var collection = PluginCollection.FromFile(Parameters.UseDefaultPlugins);
				pluginDirectories.Add(PathUtils.MakeRelativePath(
					Path.GetDirectoryName(Path.GetDirectoryName(outputPath)),
					Path.GetDirectoryName(Parameters.UseDefaultPlugins)));
				pluginDirectories.Add("./");
				plugins.AddRange(collection.PrependPlugins);
				plugins.Add(Parameters.ProjectName);
				plugins.AddRange(collection.AppendPlugins);
			}
			var json = JsonConvert.SerializeObject(new {
				Database = Parameters.Database,
				ConnectionString = Parameters.ConnectionString,
				PluginDirectories = pluginDirectories,
				Plugins = plugins
			}, Formatting.Indented);
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			File.WriteAllText(outputPath, json);
		}

		/// <summary>
		/// 写入模板内容
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <param name="outputPath">写入的文件路径</param>
		/// <param name="parameters">参数</param>
		protected virtual void WriteTemplateContents(
			string path, string outputPath, object parameters) {
			var contents = File.ReadAllText(path);
			foreach (var property in parameters.GetType().GetTypeInfo().GetProperties()) {
				var expr = "${" + property.Name + "}";
				contents = contents.Replace(expr, property.GetValue(parameters).ToString());
			}
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			File.WriteAllText(outputPath, contents);
		}

		/// <summary>
		/// 创建项目
		/// </summary>
		public virtual void CreateProject() {
			// 获取储存模板的文件夹
			var templatesDirectory = Parameters.TemplatesDirectory ?? AutoDetectTemplatesDirectory();
			// 获取项目模板路径和插件模板路径
			var projectTemplateName = Parameters.ProjectType + "Template";
			var pluginTemplateName = "PluginTemplate";
			var projectTemplateRoot = Path.Combine(templatesDirectory, projectTemplateName);
			var pluginTemplateRoot = Path.Combine(templatesDirectory, pluginTemplateName);
			var outputRoot = Path.Combine(Parameters.OutputDirectory, Parameters.ProjectName);
			// 创建模板参数
			var random = new Random();
			var templateParameters = new {
				ProjectName = Parameters.ProjectName,
				ProjectNameLower = Parameters.ProjectName.ToLower(),
				ProjectDescription = Parameters.ProjectDescription,
				IISPort = random.Next(50000, 64999),
				SelfHostPort = random.Next(40000, 49999),
				WebProjectGuid = Guid.NewGuid(),
				ConsoleProjectGuid = Guid.NewGuid()
			};
			// 写入项目文件
			foreach (var path in Directory.EnumerateFiles(
				projectTemplateRoot, "*", SearchOption.AllDirectories)) {
				var relPath = path.Substring(projectTemplateRoot.Length + 1);
				var outputPath = Path.Combine(outputRoot,
					relPath.Replace(projectTemplateName, Parameters.ProjectName));
				if (Path.GetFileName(outputPath) == "config.json") {
					WriteConfigObject(outputPath);
				} else {
					WriteTemplateContents(path, outputPath, templateParameters);
				}
			}
			// 写入插件文件
			var webRoot = Path.GetDirectoryName(Directory.EnumerateDirectories(
				outputRoot, "App_Data", SearchOption.AllDirectories).First());
			foreach (var path in Directory.EnumerateFiles(
				pluginTemplateRoot, "*", SearchOption.AllDirectories)) {
				var relPath = path.Substring(pluginTemplateRoot.Length + 1);
				var outputPath = Path.Combine(webRoot, Parameters.ProjectName,
					relPath.Replace(pluginTemplateName.ToLower(), Parameters.ProjectName.ToLower()));
				WriteTemplateContents(path, outputPath, templateParameters);
			}
		}
	}
}
