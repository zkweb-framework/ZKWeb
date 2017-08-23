using Mono.Options;
using System;
using System.IO;
using ZKWeb.Toolkits.WebsitePublisher.Cmd.Properties;
using ZKWeb.Toolkits.WebsitePublisher.Model;

namespace ZKWeb.Toolkits.WebsitePublisher.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new PublishWebsiteParameters();
			var showHelp = false;
			var optionSet = new OptionSet() {
				{ "r|root=", Resources.TheSourceWebsiteRootDirectory, r => parameters.WebRoot = r },
				{ "n|name=", Resources.TheOutputName, n => parameters.OutputName = n },
				{ "o|output=", Resources.TheOutputDirectory, d => parameters.OutputDirectory = d },
				{ "x|ignore=", Resources.IgnorePatternInRegex, x => parameters.IgnorePattern = x },
				{ "c|configuration=", Resources.WhichConfigurationToPublish,x => parameters.Configuration = x },
				{ "f|framework=", Resources.WhichFrameworkToPublish, x => parameters.Framework = x },
				{ "h|help", Resources.ShowThisMessageAndExit, h => showHelp = (h != null) }
			};
			try {
				optionSet.Parse(args);
				if (showHelp) {
					var optionDescriptions = new StringWriter();
					optionSet.WriteOptionDescriptions(optionDescriptions);
					Console.WriteLine("WebsitePublisher 2.0:");
					Console.WriteLine(optionDescriptions.ToString());
					return;
				}
				var publisher = new WebsitePublisher(parameters);
				publisher.PublishWebsite();
				Console.WriteLine("success");
			} catch (Exception e) {
				if (e is OptionException || e is ArgumentException) {
					Console.WriteLine(e.Message);
					Console.WriteLine(Resources.TryWebsitePublisherHelpForMoreInformation);
				} else {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
