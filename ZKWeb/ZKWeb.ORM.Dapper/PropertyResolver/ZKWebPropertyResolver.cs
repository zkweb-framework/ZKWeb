using Dapper.FluentMap.Dommel.Resolvers;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.Dapper.PropertyResolver {
	/// <summary>
	/// Replace dommel property resolver<br/>
	/// don't filter complex types<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	internal class ZKWebPropertyResolver : DommelPropertyResolver {
		/// <summary>
		/// Don't filter complex types<br/>
		/// <br/>
		/// </summary>
		protected override IEnumerable<PropertyInfo> FilterComplexTypes(IEnumerable<PropertyInfo> properties) {
			return properties;
		}
	}
}
