using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Templating.AreaSupport {
	/// <summary>
	/// 模板区域的扩展函数
	/// </summary>
	public static class TemplateAreaExtensions {
		/// <summary>
		/// 添加模块
		/// </summary>
		/// <param name="widgets">模块列表</param>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <param name="args">模块参数</param>
		public static void Add(
			this List<TemplateWidget> widgets, string path, object args = null) {
			widgets.Add(new TemplateWidget(path, args));
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
			this List<TemplateWidget> widgets, string beforePath, string path, object args = null) {
			widgets.AddBefore(x => x.Path == beforePath, new TemplateWidget(path, args));
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
			this List<TemplateWidget> widgets, string afterPath, string path, object args = null) {
			widgets.AddAfter(x => x.Path == afterPath, new TemplateWidget(path, args));
		}
	}
}
