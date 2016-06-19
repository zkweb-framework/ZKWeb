namespace ZKWeb.Web {
	/// <summary>
	/// Http请求的处理器接口
	/// 在IHttpRequestPreHandler调用后调用，后注册的先调用
	/// </summary>
	public interface IHttpRequestHandler {
		/// <summary>
		/// 处理Http请求
		/// 结束回应可以抛出HttpResultException例外
		/// </summary>
		void OnRequest();
	}
}
