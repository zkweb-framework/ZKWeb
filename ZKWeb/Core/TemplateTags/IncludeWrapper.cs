using DotLiquid.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotLiquid;
using System.IO;

namespace ZKWeb.Core.TemplateTags {
	/// <summary>
	/// 对默认Include标签的包装
	/// 会在模板上下文中设置引用文件的路径
	/// </summary>
	public class IncludeWrapper : Include {
		/// <summary>
		/// 保存当前嵌入文件路径时使用的键名
		/// </summary>
		public static string CurrentFilePathKey { get; set; } = "__current_file_path";

		/// <summary>
		/// 描画内容
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			var path = this.Markup.Trim();
			var pathEncoded = HttpUtility.HtmlEncode(path);
			// 添加当前文件路径的变量
			context.Push(Hash.FromDictionary(new Dictionary<string, object>() {
				{ CurrentFilePathKey, path }
			}));
			// 添加开头注释
			result.Write($"<!--include {pathEncoded}-->");
			// 原有的处理
			base.Render(context, result);
			context.Stack(() => {
				// 删除当前文件路径的变量
				context.Pop();
				// 添加末尾注释
				result.Write($"<!--end include {pathEncoded}-->");
			});
		}
	}
}
