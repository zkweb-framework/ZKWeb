using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DotLiquid;

namespace ZKWeb.Core.TemplateTags {
	/// <summary>
	/// 用于提供动态内容的区域
	/// 例子
	/// 内容可以在插件中动态注册，见DiyManager
	/// {% area test_area %}
	///		{% default_widgets %}
	/// {% endarea %}
	/// 
	/// 例子
	/// 固定的模块，可以手动或在可视化编辑中修改
	/// {% area test_area %}
	///		{% widget logo %}
	///		{% widget test_widget %}
	/// {% endarea %}
	/// 
	/// 生成的Html例子（使用[]代替）
	/// 这里的diy_area和area_id都会作为可视化编辑时判断是否区域的依据
	/// [div class='diy_area' area_id='test_area' file='include/header.html']
	///		[div class='diy_widget' widget_id='random_123123123'][/div]
	///		[div class='diy_widget' widget_id='random_123123122'][/div]
	///		[div class='diy_widget' widget_id='random_123123121'][/div]
	/// [/div]
	/// </summary>
	public class Area : Block {
		/// <summary>
		/// 保存当前区域Id时使用的键名
		/// </summary>
		public static string CurrentAreaIdKey { get; set; } = "__current_area_id";

		/// <summary>
		/// 描画标签
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			var areaId = this.Markup.Trim();
			// 区域不能嵌套
			if (context[CurrentAreaIdKey] != null) {
				throw new FormatException("area tag can't be nested");
			}
			// 添加div的开头
			result.Write($"<div class='diy_area' area_id='{areaId}' >");
			// 添加当前区域Id的变量
			context.Push(Hash.FromDictionary(new Dictionary<string, object>() {
				{ CurrentAreaIdKey, areaId }
			}));
			context.Stack(() => {
				// 描画子元素
				RenderAll(NodeList, context, result);
				// 删除当前区域Id的变量
				context.Pop();
				// 添加div的末尾
				result.Write("</div>");
			});
		}
	}
}
