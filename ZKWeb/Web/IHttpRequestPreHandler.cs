namespace ZKWeb.Web {
	/// <summary>
	/// Http请求的预处理器接口
	/// 在IHttpRequestHandler调用前调用，先注册的先调用
	/// </summary>
	public interface IHttpRequestPreHandler {
		/// <summary>
		/// 处理Http请求
		/// </summary>
		void OnRequest();
	}
}
