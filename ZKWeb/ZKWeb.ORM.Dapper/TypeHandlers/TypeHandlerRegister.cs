using Dapper;
using System;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Dapper type handler registrator
	/// </summary>
	internal static class TypeHandlerRegistrator {
		/// <summary>
		/// Register dapper type handler
		/// </summary>
		public static void Register(Type type, SqlMapper.ITypeHandler handler) {
			// Dapper will replace exists handler, and no need to clone typeHandlers
			SqlMapper.AddTypeHandlerImpl(type, handler, false);
		}
	}
}
