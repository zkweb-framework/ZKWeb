using Microsoft.Owin.Hosting;
using System;

namespace ZKWeb.Owin.SelfHost {
	class Program {
		static void Main(string[] args) {
			var url = "http://localhost:5000";
			using (WebApp.Start<Startup>(url)) {
				Console.WriteLine(string.Format("Open {0} in the browser or", url));
				Console.WriteLine("Press any key to continue..");
				Console.ReadLine();
			}
		}
	}
}
