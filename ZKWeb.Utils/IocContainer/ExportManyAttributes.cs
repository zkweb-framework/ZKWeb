using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// 导出类型到自身类型和继承的类型
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ExportManyAttributes : Attribute {
		/// <summary>
		/// 关联键
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// 排除的类型列表
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// 是否包含私有类型，默认不包含
		/// </summary>
		public bool NonPublic { get; set; }
	}
}
