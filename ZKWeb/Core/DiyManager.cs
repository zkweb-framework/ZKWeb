using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Core {
	/// <summary>
	/// 主要提供区域(Area)和模块(Widget)和可视化编辑的管理
	/// 这个管理器和TemplateManager不同的是，
	/// TemplateManager管理的是模板文件本身而这个类针对Area和Widget标签管理
	/// </summary>
	public class DiyManager {
		/// <summary>
		/// 已知的区域列表
		/// </summary>
		private Dictionary<string, DiyArea> Areas { get; } =
			new Dictionary<string, DiyArea>();

		/// <summary>
		/// 获取区域
		/// 这个函数不会返回null
		/// </summary>
		/// <param name="id">区域Id</param>
		/// <returns></returns>
		public DiyArea GetArea(string id) {
			return Areas.GetOrCreate(id, () => new DiyArea(id));
		}
	}
}
