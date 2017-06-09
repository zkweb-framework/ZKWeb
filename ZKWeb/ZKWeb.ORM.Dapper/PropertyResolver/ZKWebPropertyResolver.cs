using Dapper.FluentMap.Dommel.Resolvers;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.Dapper.PropertyResolver {
	/// <summary>
	/// Replace dommel property resolver<br/>
	/// don't filter complex types<br/>
	/// 替换dommel的属性解决器<br/>
	/// 不过滤复杂类型<br/>
	/// </summary>
	public class ZKWebPropertyResolver : DommelPropertyResolver {
		/// <summary>
		/// Don't filter complex types<br/>
		/// 不过滤复杂类型<br/>
		/// </summary>
		protected override IEnumerable<PropertyInfo> FilterComplexTypes(IEnumerable<PropertyInfo> properties) {
			return properties;
		}
	}
}
