using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Model {
	/// <summary>
	/// 网站启动时的处理器接口
	/// 在所有插件初始化完成后调用
	/// </summary>
	public interface IWebsiteStartHandler {
		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		void OnWebsiteStart();
	}
}
