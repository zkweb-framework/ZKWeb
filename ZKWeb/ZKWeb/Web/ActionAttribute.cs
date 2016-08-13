using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Mark action method in controller
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionAttribute : Attribute {
		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Method, GET or POST or whatever
		/// </summary>
		public string Method { get; set; }
		/// <summary>
		/// Allow override exist actions
		/// </summary>
		public bool OverrideExists { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method, default is GET</param>
		public ActionAttribute(string path, string method = HttpMethods.GET) {
			Path = path;
			Method = method;
			OverrideExists = false;
		}
	}
}
