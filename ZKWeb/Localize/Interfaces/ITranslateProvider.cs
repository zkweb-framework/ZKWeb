using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Localize.Interfaces {
	/// <summary>
	/// 翻译提供器的接口
	/// </summary>
	public interface ITranslateProvider {
		/// <summary>
		/// 判断是否支持翻译指定的语言
		/// </summary>
		bool CanTranslate(string code);

		/// <summary>
		/// 翻译文本
		/// 没有对应文本时应返回null
		/// </summary>
		/// <param name="text">文本</param>
		/// <returns></returns>
		string Translate(string text);
	}
}
