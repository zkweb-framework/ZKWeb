using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// 对象的重用策略
	/// </summary>
	public enum ReuseType {
		/// <summary>
		/// 即时模式，不共享对象
		/// </summary>
		Transient = 0,
		/// <summary>
		/// 单例模式，共享并重用对象
		/// </summary>
		Singleton = 1,
	}
}
