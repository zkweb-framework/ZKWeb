namespace ZKWebStandard.Ioc {
	/// <summary>
	/// 解决失败时的处理
	/// </summary>
	public enum IfUnresolved {
		/// <summary>
		/// 抛出例外
		/// </summary>
		Throw = 0,
		/// <summary>
		/// 返回默认值
		/// </summary>
		ReturnDefault = 1
	}
}
