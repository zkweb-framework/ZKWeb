using Mono.Options;
using System;
using System.IO;
using ZKWeb.Toolkits.ProjectCreator.Cmd.Properties;
using ZKWeb.Toolkits.ProjectCreator.Model;

namespace ZKWeb.Toolkits.ProjectCreator.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new CreateProjectParameters();
			var showHelp = false;
			var optionSet = new OptionSet() {
				{ "t|type=", Resources.AspNetAspNetCoreOrOwin, t => parameters.ProjectType = t },
				{ "n|name=", Resources.ProjectName, n => parameters.ProjectName = n },
				{ "d|description=", Resources.ProjectDescription , d => parameters.ProjectDescription = d },
				{ "m|orm=", Resources.DapperEFCoreMongoDBOrNHibernate, m => parameters.ORM = m },
				{ "b|database=", Resources.MSSQLMySQLSQLiteMongoDBOrPostgreSQL, d => parameters.Database = d },
				{ "c|connectionString=", Resources.TheConnectionString, c => parameters.ConnectionString = c },
				{ "u|useDefaultPlugins=", Resources.TheLocationOfPluginCollectionJson, u => parameters.UseDefaultPlugins = u },
				{ "o|output=", Resources.OutputDirectory, o => parameters.OutputDirectory = o },
				{ "h|help", Resources.ShowThisMessageAndExit, h => showHelp = (h != null) }
			};
			try {
				optionSet.Parse(args);
				if (showHelp) {
					var optionDescriptions = new StringWriter();
					optionSet.WriteOptionDescriptions(optionDescriptions);
					Console.WriteLine("ProjectCreator 2.0:");
					Console.WriteLine(optionDescriptions.ToString());
					return;
				}
				var creator = new ProjectCreator(parameters);
				creator.CreateProject();
				Console.WriteLine("success");
			} catch (Exception e) {
				if (e is OptionException || e is ArgumentException) {
					Console.WriteLine(e.Message);
					Console.WriteLine(Resources.TryProjectCreatorHelpForMoreInformation);
				} else {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
