using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Localize.Interfaces {
	/// <summary>
	/// 语言的接口
	/// </summary>
	public interface ILanguage {
		/// <summary>
		/// 语言名称，格式ISO 639 xx-XX
		/// </summary>
		string Name { get; }
	}
}
