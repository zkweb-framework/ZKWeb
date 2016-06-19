using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// 设置Cookies时使用的选项
	/// </summary>
	public class HttpCookieOptions {
		/// <summary>
		/// 域名，不设置时使用默认值
		/// </summary>
		public virtual string Domain { get; set; }
		/// <summary>
		/// 过期时间，不设置时在浏览器关闭后过期
		/// </summary>
		public virtual DateTime? Expires { get; set; }
		/// <summary>
		/// 是否不允许脚本获取，不设置时使用默认值
		/// </summary>
		public virtual bool? HttpOnly { get; set; }
		/// <summary>
		/// 路径，不设置时使用默认值
		/// </summary>
		public virtual string Path { get; set; }
		/// <summary>
		/// 是否只在https中使用，不设置时使用默认值
		/// </summary>
		public virtual bool? Secure { get; set; }
	}
}
