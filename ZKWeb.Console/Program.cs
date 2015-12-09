namespace ZKWeb.Console {
	using DotLiquid;
	using DotLiquid.FileSystems;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web;

	class Program {
		static void Main(string[] args) {
			new Application().Application_Start();
			
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
