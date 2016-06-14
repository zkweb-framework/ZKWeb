using FluentNHibernate.Cfg;

namespace ZKWeb.Database {
	/// <summary>
	/// 数据库初始化处理器
	/// </summary>
	public interface IDatabaseInitializeHandler {
		/// <summary>
		/// 数据库初始化时的处理
		/// </summary>
		/// <param name="configuration"></param>
		void OnInitialize(FluentConfiguration configuration);
	}
}
