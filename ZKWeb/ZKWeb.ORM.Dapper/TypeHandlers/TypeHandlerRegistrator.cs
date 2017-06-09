using Dapper;
using System;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Dapper type handler registrator<br/>
	/// 注册dapper类型处理器<br/>
	/// </summary>
	public static class TypeHandlerRegistrator {
		/// <summary>
		/// Register dapper type handler<br/>
		/// 注册dapper类型处理器<br/>
		/// </summary>
		public static void Register(Type type, SqlMapper.ITypeHandler handler) {
			// Dapper will replace exists handler, and no need to clone typeHandlers
			SqlMapper.AddTypeHandlerImpl(type, handler, false);
		}

		/// <summary>
		/// Register dapper type handler for json serialized type<br/>
		/// 注册dapper类型处理器, 用于json序列化的类型<br/>
		/// </summary>
		/// <param name="type">Serialized type</param>
		public static void RegisterJsonSerializedType(Type type) {
			Register(type,
				(SqlMapper.ITypeHandler)Activator.CreateInstance(
				typeof(JsonSerializedTypeHandler<>).MakeGenericType(type)));
		}
	}
}
