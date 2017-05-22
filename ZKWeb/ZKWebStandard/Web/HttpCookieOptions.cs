using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Options for setting cookie<br/>
	/// <br/>
	/// </summary>
	public class HttpCookieOptions {
		/// <summary>
		/// Domain, use defualt value if empty<br/>
		/// <br/>
		/// </summary>
		public virtual string Domain { get; set; }
		/// <summary>
		/// Expire time, expire after close browser if null<br/>
		/// <br/>
		/// </summary>
		public virtual DateTime? Expires { get; set; }
		/// <summary>
		/// Disallow getting this cookie from javascript, use default value if null<br/>
		/// <br/>
		/// </summary>
		public virtual bool? HttpOnly { get; set; }
		/// <summary>
		/// Path, use default value if empty<br/>
		/// <br/>
		/// </summary>
		public virtual string Path { get; set; }
		/// <summary>
		/// Only send for https connection, use default value if null<br/>
		/// <br/>
		/// </summary>
		public virtual bool? Secure { get; set; }
	}
}
