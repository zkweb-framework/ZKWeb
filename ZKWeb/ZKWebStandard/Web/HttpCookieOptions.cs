using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Options for setting cookie<br/>
	/// Http Cookie的设置选项<br/>
	/// </summary>
	public class HttpCookieOptions {
		/// <summary>
		/// Domain, use defualt value if empty<br/>
		/// 域名, 空时使用默认值<br/>
		/// </summary>
		public virtual string Domain { get; set; }
		/// <summary>
		/// Expire time, expire after close browser if null<br/>
		/// 过期时间, null时表示浏览器关闭后过期<br/>
		/// </summary>
		public virtual DateTime? Expires { get; set; }
		/// <summary>
		/// Disallow getting this cookie from javascript, use default value if null<br/>
		/// 是否禁止js获取此Cookie值, null时使用默认值<br/>
		/// </summary>
		public virtual bool? HttpOnly { get; set; }
		/// <summary>
		/// Path, use default value if empty<br/>
		/// 路径, 空时使用默认值<br/>
		/// </summary>
		public virtual string Path { get; set; }
		/// <summary>
		/// Only send for https connection, use default value if null<br/>
		/// 是否只在https连接中发送, null时使用默认值<br/>
		/// </summary>
		public virtual bool? Secure { get; set; }
	}
}
