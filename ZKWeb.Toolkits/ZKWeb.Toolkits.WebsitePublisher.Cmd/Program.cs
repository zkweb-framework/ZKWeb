using Mono.Options;
using System;
using System.IO;
using ZKWeb.Toolkits.WebsitePublisher.Model;

namespace ZKWeb.Toolkits.WebsitePublisher.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new PublishWebsiteParameters();
			var showHelp = false;
			var optionSet = new OptionSet() {
				{ "r|root=", "The source website root directory", r => parameters.WebRoot = r },
				{ "n|name=", "The output name", n => parameters.OutputName = n },
				{ "o|output=", "The output directory", d => parameters.OutputDirectory = d },
				{ "h|help", "Show this message and exit", h => showHelp = (h != null) }
			};
			try {
				optionSet.Parse(args);
				if (showHelp) {
					var optionDescriptions = new StringWriter();
					optionSet.WriteOptionDescriptions(optionDescriptions);
					Console.WriteLine("WebsitePublisher:");
					Console.WriteLine(optionDescriptions.ToString());
					return;
				}
				var publisher = new WebsitePublisher(parameters);
				publisher.PublishWebsite();
				Console.WriteLine("success");
			} catch (Exception e) {
				if (e is OptionException || e is ArgumentException) {
					Console.WriteLine(e.Message);
					Console.WriteLine("Try `WebsitePublisher --help` for more information");
				} else {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
