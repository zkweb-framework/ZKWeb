using System.Collections.Generic;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area extension methods<br/>
	/// 模板区域的扩展函数<br/>
	/// </summary>
	/// <seealso cref="TemplateArea"/>
	public static class TemplateAreaExtensions {
		/// <summary>
		/// Add widget<br/>
		/// 向模块列表中添加模块<br/>
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void Add(
			this IList<TemplateWidget> widgets, string path, object args = null) {
			widgets.Add(new TemplateWidget(path, args));
		}

		/// <summary>
		/// Add widget before the specified widget<br/>
		/// If specified widget not found then add it to the front<br/>
		/// 在指定模块前插入模块<br/>
		/// 如果指定模块不存在则插入到开头<br/>
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="beforePath">Add before widget that path equals it</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void AddBefore(
			this IList<TemplateWidget> widgets, string beforePath, string path, object args = null) {
			widgets.AddBefore(x => x.Path == beforePath, new TemplateWidget(path, args));
		}

		/// <summary>
		/// Add widget after the specified widget<br/>
		/// If specified widget not found then add it to the back<br/>
		/// 在指定模块后插入模块<br/>
		/// 如果指定模块不存在则插入到结尾<br/>
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="afterPath">Add after widget that path equals it</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void AddAfter(
			this IList<TemplateWidget> widgets, string afterPath, string path, object args = null) {
			widgets.AddAfter(x => x.Path == afterPath, new TemplateWidget(path, args));
		}
	}
}
