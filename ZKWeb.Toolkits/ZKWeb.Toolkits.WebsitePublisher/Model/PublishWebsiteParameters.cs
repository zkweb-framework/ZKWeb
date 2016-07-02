using System;

namespace ZKWeb.Toolkits.WebsitePublisher.Model {
	/// <summary>
	/// 发布网站的参数
	/// </summary>
	public class PublishWebsiteParameters {
		/// <summary>
		/// 网站根目录
		/// </summary>
		public string WebRoot { get; set; }
		/// <summary>
		/// 发布网站的名称
		/// </summary>
		public string OutputName { get; set; }
		/// <summary>
		/// 发布到的目录
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// 检查参数
		/// </summary>
		public void Check() {
			if (string.IsNullOrEmpty(WebRoot)) {
				throw new ArgumentException("WebRoot can't be empty");
			} else if (string.IsNullOrEmpty(OutputName)) {
				throw new ArgumentException("OutputName can't be empty");
			} else if (string.IsNullOrEmpty(OutputDirectory)) {
				throw new ArgumentException("OutputDirectory can't be empty");
			}
		}
	}
}
