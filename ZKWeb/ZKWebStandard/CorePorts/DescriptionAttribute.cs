#if NETCORE
namespace System.ComponentModel {
	/// <summary>
	/// 描述属性
	/// 参考:
	/// https://github.com/dotnet/corefx/issues/5625
	/// 进度:
	/// 预计RTM可移除
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute {
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="description">描述</param>
		public DescriptionAttribute(string description) {
			Description = description;
		}
	}
}
#endif
