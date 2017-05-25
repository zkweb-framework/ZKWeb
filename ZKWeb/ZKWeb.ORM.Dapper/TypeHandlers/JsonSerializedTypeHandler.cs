using Dapper;
using Newtonsoft.Json;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Handle json serialized type<br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="T">Json serialized type</typeparam>
	internal class JsonSerializedTypeHandler<T> : SqlMapper.StringTypeHandler<T>
		where T : new() {
		/// <summary>
		/// Parse from json<br/>
		/// <br/>
		/// </summary>
		protected override T Parse(string value) {
			var json = value?.ToString();
			if (string.IsNullOrEmpty(json)) {
				return new T();
			}
			return JsonConvert.DeserializeObject<T>(value?.ToString());
		}

		/// <summary>
		/// Serialize to json<br/>
		/// <br/>
		/// </summary>
		protected override string Format(T value) {
			return JsonConvert.SerializeObject(value);
		}
	}
}
