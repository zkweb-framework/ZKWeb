using Dapper.FluentMap.Dommel.Resolvers;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.Dapper.PropertyResolver {
	/// <summary>
	/// Replace dommel property resolver
	/// don't filter complex types
	/// </summary>
	internal class ZKWebPropertyResolver : DommelPropertyResolver {
		/// <summary>
		/// Don't filter complex types
		/// </summary>
		protected override IEnumerable<PropertyInfo> FilterComplexTypes(IEnumerable<PropertyInfo> properties) {
			return properties;
		}
	}
}
