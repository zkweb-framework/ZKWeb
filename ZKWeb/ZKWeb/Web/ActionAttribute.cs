using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Mark action method in controller<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionAttribute : Attribute {
		/// <summary>
		/// Path<br/>
		/// <br/>
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Method, GET or POST or whatever<br/>
		/// <br/>
		/// </summary>
		public string Method { get; set; }
		/// <summary>
		/// Allow override exist actions<br/>
		/// <br/>
		/// </summary>
		public bool OverrideExists { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
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
