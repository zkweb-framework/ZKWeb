using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using ZKWeb.Toolkits.ProjectCreator.Model;
using ZKWeb.Toolkits.ProjectCreator.Utils;
using System.Linq;
using System.Text;
using ZKWeb.Toolkits.ProjectCreator.Properties;

namespace ZKWeb.Toolkits.ProjectCreator {
	/// <summary>
	/// ZKWeb project creator
	/// </summary>
	public class ProjectCreator {
		/// <summary>
		/// Create project parameters
		/// </summary>
		public CreateProjectParameters Parameters { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="parameters">Create project parameters</param>
		public ProjectCreator(CreateProjectParameters parameters) {
			parameters.Check();
			Parameters = parameters;
		}

		/// <summary>
		/// Automatic detect templates directory
		/// </summary>
		/// <returns></returns>
		protected virtual string AutoDetectTemplatesDirectory() {
			var path = Path.GetDirectoryName(typeof(ProjectCreator).GetTypeInfo().Assembly.Location);
			while (!Directory.Exists(Path.Combine(path, "Tools"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException(Resources.DetectTemplatesDirectoryFailed);
				}
			}
			return Path.Combine(path, "Tools", "Templates");
		}

		/// <summary>
		/// Write config.json
		/// </summary>
		/// <param name="outputPath">Path of config.json</param>
		protected virtual void WriteConfigObject(string outputPath) {
			var pluginDirectories = new List<string>();
			var plugins = new List<string>();
			var pluginRoot = $"../{Parameters.ProjectName}.Plugins";
			if (string.IsNullOrEmpty(Parameters.UseDefaultPlugins)) {
				// Not use default plugins
				pluginDirectories.Add(pluginRoot);
				plugins.Add(Parameters.ProjectName);
			} else {
				// Use default plugins
				var collection = PluginCollection.FromFile(Parameters.UseDefaultPlugins);
				pluginDirectories.Add(pluginRoot);
				pluginDirectories.Add(PathUtils.MakeRelativePath(
					Path.GetDirectoryName(Path.GetDirectoryName(outputPath)),
					Path.GetDirectoryName(Parameters.UseDefaultPlugins)));
				plugins.AddRange(collection.PrependPlugins);
				plugins.Add(Parameters.ProjectName);
				plugins.AddRange(collection.AppendPlugins);
			}
			var json = JsonConvert.SerializeObject(new {
				ORM = Parameters.ORM,
				Database = Parameters.Database,
				ConnectionString = Parameters.ConnectionString,
				PluginDirectories = pluginDirectories,
				Plugins = plugins
			}, Formatting.Indented);
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			File.WriteAllText(outputPath, json, Encoding.UTF8);
		}

		/// <summary>
		/// Write template contents
		/// </summary>
		/// <param name="path">Template path</param>
		/// <param name="outputPath">Project output path</param>
		/// <param name="parameters">Replace parameters</param>
		protected virtual void WriteTemplateContents(
			string path, string outputPath, IDictionary<string, string> parameters) {
			var contents = File.ReadAllText(path);
			foreach (var pair in parameters) {
				contents = contents.Replace(pair.Key, pair.Value);
			}
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			File.WriteAllText(outputPath, contents, Encoding.UTF8);
		}

		/// <summary>
		/// Create proejct
		/// </summary>
		public virtual void CreateProject() {
			// Get templates directory
			var templatesDirectory = Parameters.TemplatesDirectory ?? AutoDetectTemplatesDirectory();
			// Get project template path and plugin template path
			var projectNameInPath = "__ProjectName__";
			var projectTemplateName = $"{Parameters.ProjectType}.{Parameters.ORM}";
			var pluginTemplateName = "BootstrapPlugin";
			var projectTemplateRoot = Path.Combine(templatesDirectory, projectTemplateName);
			var pluginTemplateRoot = Path.Combine(templatesDirectory, pluginTemplateName);
			var outputRoot = Path.Combine(Parameters.OutputDirectory, Parameters.ProjectName);
			// Create replace parameters
			var random = new Random();
			var iisPort = random.Next(50000, 59999);
			var selfHostPort = random.Next(40000, 49999);
			var webProjectGuid = Guid.NewGuid();
			var consoleProjectGuid = Guid.NewGuid();
			var pluginProjectGuid = Guid.NewGuid();
			var templateParameters = new Dictionary<string, string>() {
				{ "__ProjectName__", Parameters.ProjectName },
				{ "__ProjectDescription__" , Parameters.ProjectDescription },
				{ "<DevelopmentServerPort>50000</DevelopmentServerPort>",
					$"<DevelopmentServerPort>{iisPort}</DevelopmentServerPort>" },
				{ "<IISUrl>http://localhost:50000/</IISUrl>",
					$"<IISUrl>http://localhost:{iisPort}/</IISUrl>" },
				{ "\"applicationUrl\": \"http://localhost:50000/\"",
					$"\"applicationUrl\": \"http://localhost:{iisPort}/\"" },
				{ "\"launchUrl\": \"http://localhost:40000\"",
					$"\"launchUrl\": \"http://localhost:{selfHostPort}\"" },
				{ "fc8611cf-a950-4fc1-bf0e-cbe87b820900",
					webProjectGuid.ToString() },
				{ "fc8611cf-a950-4fc1-bf0e-cbe87b820901",
					consoleProjectGuid.ToString() },
				{ "fc8611cf-a950-4fc1-bf0e-cbe87b820902",
					pluginProjectGuid.ToString() },
			};
			// Write project files
			foreach (var path in Directory.EnumerateFiles(
				projectTemplateRoot, "*", SearchOption.AllDirectories)) {
				var relPath = path.Substring(projectTemplateRoot.Length + 1);
				var outputPath = Path.Combine(outputRoot,
					relPath.Replace(projectNameInPath, Parameters.ProjectName));
				if (Path.GetFileName(outputPath) == "config.json") {
					WriteConfigObject(outputPath);
				} else {
					WriteTemplateContents(path, outputPath, templateParameters);
				}
			}
			// Write plugin files
			var pluginRoot = Directory.EnumerateDirectories(
				outputRoot, "*.Plugins", SearchOption.AllDirectories).First();
			foreach (var path in Directory.EnumerateFiles(
				pluginTemplateRoot, "*", SearchOption.AllDirectories)) {
				var relPath = path.Substring(pluginTemplateRoot.Length + 1);
				var outputPath = Path.Combine(pluginRoot, Parameters.ProjectName,
					relPath.Replace(projectNameInPath, Parameters.ProjectName));
				WriteTemplateContents(path, outputPath, templateParameters);
			}
		}
	}
}
