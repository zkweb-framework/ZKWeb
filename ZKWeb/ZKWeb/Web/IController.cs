namespace ZKWeb.Web {
	/// <summary>
	/// Interface of the controller<br/>
	/// 控制器的接口<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// class TestController : IController {
	///		[Action("index.html")]
	///		public string Index() { return "test index"; }
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="ControllerManager"/>
	public interface IController {
	}
}
