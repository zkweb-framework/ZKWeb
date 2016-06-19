using System;
using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http回应的接口
	/// </summary>
	public interface IHttpResponse {
		/// <summary>
		/// 回应的数据流
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// 内容类型
		/// </summary>
		string ContentType { get; set; }
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// 状态代码
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// 设置Cookie值
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="value">Cookie值</param>
		/// <param name="options">使用的选项</param>
		void SetCookie(string key, string value, HttpCookieOptions options);
		/// <summary>
		/// 添加Http头的值
		/// </summary>
		/// <param name="key">键名</param>
		/// <param name="value">值</param>
		void AddHeader(string key, string value);
		/// <summary>
		/// 跳转到指定的地址
		/// </summary>
		/// <param name="url">Url地址</param>
		/// <param name="permanent">是否永久跳转</param>
		void Redirect(string url, bool permanent);
		/// <summary>
		/// 结束回应
		/// </summary>
		void End();
	}
}
