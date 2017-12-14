using Dapper;
using Newtonsoft.Json;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Handle json serialized type<br/>
	/// 处理json序列化的类型<br/>
	/// </summary>
	/// <typeparam name="T">Json serialized type</typeparam>
	public class JsonSerializedTypeHandler<T> : SqlMapper.StringTypeHandler<T>
		where T : new() {
#pragma warning disable S927 // parameter names should match base declaration and other partial definitions
		/// <summary>
		/// Parse from json<br/>
		/// 从json解析<br/>
		/// </summary>
		protected override T Parse(string value) {
#pragma warning restore S927 // parameter names should match base declaration and other partial definitions
			var json = value?.ToString();
			if (string.IsNullOrEmpty(json)) {
				return new T();
			}
			return JsonConvert.DeserializeObject<T>(value?.ToString());
		}

#pragma warning disable S927 // parameter names should match base declaration and other partial definitions
		/// <summary>
		/// Serialize to json<br/>
		/// 序列化到json<br/>
		/// </summary>
		protected override string Format(T value) {
#pragma warning restore S927 // parameter names should match base declaration and other partial definitions
			return JsonConvert.SerializeObject(value);
		}
	}
}
