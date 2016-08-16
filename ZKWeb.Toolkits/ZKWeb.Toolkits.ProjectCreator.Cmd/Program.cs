using Mono.Options;
using System;
using System.IO;
using ZKWeb.Toolkits.ProjectCreator.Model;

namespace ZKWeb.Toolkits.ProjectCreator.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new CreateProjectParameters();
			var showHelp = false;
			var optionSet = new OptionSet() {
				{ "t|type=", "AspNet, AspNetCore or Owin", t => parameters.ProjectType = t },
				{ "n|name=", "Project name", n => parameters.ProjectName = n },
				{ "d|description=", "Project description", d => parameters.ProjectDescription = d },
				{ "m|orm=", "Dapper, EFCore, MongoDB or NHibernate", m => parameters.ORM = m },
				{ "b|database=", "MSSQL, MySQL, SQLite, MongoDB or PostgreSQL", d => parameters.Database = d },
				{ "c|connectionString=", "The connection string", c => parameters.ConnectionString = c },
				{ "u|useDefaultPlugins=",
					"The location of plugin.collection.json, if you want to use default plugins",
					u => parameters.UseDefaultPlugins = u },
				{ "o|output=", "Output directory", o => parameters.OutputDirectory = o },
				{ "h|help", "Show this message and exit", h => showHelp = (h != null) }
			};
			try {
				optionSet.Parse(args);
				if (showHelp) {
					var optionDescriptions = new StringWriter();
					optionSet.WriteOptionDescriptions(optionDescriptions);
					Console.WriteLine("ProjectCreator:");
					Console.WriteLine(optionDescriptions.ToString());
					return;
				}
				var creator = new ProjectCreator(parameters);
				creator.CreateProject();
				Console.WriteLine("success");
			} catch (Exception e) {
				if (e is OptionException || e is ArgumentException) {
					Console.WriteLine(e.Message);
					Console.WriteLine("Try `ProjectCreator --help` for more information");
				} else {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
