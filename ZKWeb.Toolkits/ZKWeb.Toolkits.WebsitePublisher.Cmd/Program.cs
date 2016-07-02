using System;
using ZKWeb.Toolkits.WebsitePublisher.Model;

namespace ZKWeb.Toolkits.WebsitePublisher.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new PublishWebsiteParameters();
			parameters.WebRoot = @"D:\tmp\projects\Test.AspNet\Test.AspNet";
			parameters.OutputName = "Test.AspNet";
			parameters.OutputDirectory = @"D:\tmp\publish";
			var publisher = new WebsitePublisher(parameters);
			publisher.PublishWebsite();
			Console.WriteLine("done");
		}
	}
}
