using DotLiquid;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 翻译文本的帮助对象
	/// 实际翻译会在转换到字符串时执行，使用T类型保存文本可以用于延迟翻译
	/// 这个类可以直接转换到string，也可以使用ToString转换
	/// </summary>
	public struct T : ILiquidizable {
		/// <summary>
		/// 翻译前的文本
		/// </summary>
		private string Text { get; set; }

		/// <summary>
		/// 翻译文本
		/// </summary>
		/// <param name="text">文本</param>
		public T(string text) {
			Text = text;
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <param name="t"></param>
		public static implicit operator string(T t) {
			return t.ToString();
		}

		/// <summary>
		/// 允许描画到模板
		/// </summary>
		/// <returns></returns>
		object ILiquidizable.ToLiquid() {
			return ToString();
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			// 文本是空白时不需要翻译
			if (string.IsNullOrEmpty(Text)) {
				return Text ?? "";
			}
			// 获取当前线程的语言
			var cluture = Thread.CurrentThread.CurrentCulture;
			// 获取翻译提供器并进行翻译
			// 传入 {语言}-{地区}
			var providers = Application.Ioc.ResolveMany<ITranslateProvider>()
				.Where(p => p.CanTranslate(cluture.Name));
			// 翻译文本，先注册的后翻译
			foreach (var provider in providers.Reverse()) {
				var translated = provider.Translate(Text);
				if (translated != null) {
					return translated;
				}
			}
			// 没有找到翻译，返回原有的文本
			return Text ?? "";
		}
	}
}
