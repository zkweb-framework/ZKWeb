using Newtonsoft.Json;
using System.Collections.Generic;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget<br/>
	/// Inside the template area, use to display dynamic contents<br/>
	/// 模板模块<br/>
	/// 在模块区域之内, 用于显示动态的内容<br/>
	/// </summary>
	/// <seealso cref="TemplateArea"/>
	/// <seealso cref="TemplateAreaManager"/>
	public class TemplateWidget {
		/// <summary>
		/// Widget path<br/>
		/// 模块路径<br/>
		/// </summary>
		public string Path { get; protected set; }
		/// <summary>
		/// Widget arguments<br/>
		/// It will open a scope let widget template use these variables<br/>
		/// eg: if arguments is { a: 123 }, then render template {{ a }} will output 123<br/>
		/// 模块参数<br/>
		/// 这里的参数可以在模板中使用<br/>
		/// 例如: 如果参数是 { a: 123 }, 则描画模板 {{ a }} 会输出123<br/>
		/// </summary>
		public IDictionary<string, object> Args { get; protected set; }
		/// <summary>
		/// Serialize result of Args<br/>
		/// 模块参数的序列化结果<br/>
		/// </summary>
		[JsonIgnore]
		protected string argsJson = null;
		/// <summary>
		/// Serialize result of Args, cached<br/>
		/// 模块参数的序列化结果, 带缓存处理<br/>
		/// </summary>
		[JsonIgnore]
		public string ArgsJson {
			get {
				if (argsJson == null && Args != null) {
					argsJson = JsonConvert.SerializeObject(Args);
				}
				return argsJson;
			}
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Widget arguments</param>
		public TemplateWidget(string path, object args = null) {
			Path = path;
			Args = args.ConvertOrDefault<IDictionary<string, object>>();
		}
	}
}
