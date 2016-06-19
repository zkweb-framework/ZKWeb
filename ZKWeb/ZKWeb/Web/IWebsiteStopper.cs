namespace ZKWeb.Web {
	/// <summary>
	/// 停止网站运行的接口
	/// 需要平台提供
	/// </summary>
	public interface IWebsiteStopper {
		/// <summary>
		/// 停止网站运行
		/// </summary>
		void StopWebsite();
	}
}
