using System;
using ZKWeb.Toolkits.ProjectCreator.Model;

namespace ZKWeb.Toolkits.ProjectCreator.Cmd {
	public class Program {
		public static void Main(string[] args) {
			var parameters = new CreateProjectParameters() {
				ProjectType = "AspNetCore",
				ProjectName = "Test.AspNetCore",
				ProjectDescription = "Test Description",
				Database = "postgresql",
				ConnectionString = "Server=127.0.0.1;Port=5432;Database=zkweb;User Id=postgres;Password=123456;",
				UseDefaultPlugins = "D:\\Projects\\ZKWeb.Plugins\\plugin.collection.json",
				OutputDirectory = "D:\\tmp\\projects"
			};
			var creator = new ProjectCreator(parameters);
			creator.CreateProject();
			Console.WriteLine("done");
		}
	}
}
