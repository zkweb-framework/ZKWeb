using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Templating.Diy {
	/// <summary>
	/// 支持可视化编辑的区域
	/// </summary>
	public class DiyArea {
		/// <summary>
		/// 区域Id
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 默认的模块列表
		/// 在区域中使用{% default_widgets %}时会引用这里的所有模块
		/// </summary>
		public List<DiyWidget> DefaultWidgets { get; }
		= new List<DiyWidget>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="id"></param>
		public DiyArea(string id) {
			Id = id;
		}
	}

	/// <summary>
	/// 区域的扩展函数
	/// </summary>
	public static class DiyAreaExtensions {
		/// <summary>
		/// 添加模块
		/// </summary>
		/// <param name="widgets">模块列表</param>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <param name="args">模块参数</param>
		public static void Add(
			this List<DiyWidget> widgets, string path, object args = null) {
			widgets.Add(new DiyWidget(path, args));
		}

		/// <summary>
		/// 添加模块到指定路径的模块前面
		/// 如果没有找到则添加到最前面
		/// </summary>
		/// <param name="widgets">模块列表</param>
		/// <param name="beforePath">添加到这个路径的模块前面</param>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <param name="args">模块参数</param>
		public static void AddBefore(
			this List<DiyWidget> widgets, string beforePath, string path, object args = null) {
			widgets.AddBefore(x => x.Info.WidgetPath == beforePath, new DiyWidget(path, args));
		}

		/// <summary>
		/// 添加模块到指定路径的模块后面
		/// 如果没有找到则添加到最后面
		/// </summary>
		/// <param name="widgets">模块列表</param>
		/// <param name="afterPath">添加到这个路径的模块后面</param>
		/// <param name="path">模块路径，注意不能带前缀</param>
		/// <param name="args">模块参数</param>
		public static void AddAfter(
			this List<DiyWidget> widgets, string afterPath, string path, object args = null) {
			widgets.AddAfter(x => x.Info.WidgetPath == afterPath, new DiyWidget(path, args));
		}
	}
}
