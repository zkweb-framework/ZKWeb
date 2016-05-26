using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Web.Interfaces {
	/// <summary>
	/// 网站http请求的预处理器接口
	/// 在IHttpRequestHandler调用前调用，先注册的先调用
	/// </summary>
	public interface IHttpRequestPreHandler {
		/// <summary>
		/// 网站接收到http请求时的处理
		/// </summary>
		void OnRequest();
	}
}
