using Dapper;
using Newtonsoft.Json;
using System.Data;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Handle json serialized type
	/// </summary>
	/// <typeparam name="T">Json serialized type</typeparam>
	internal class JsonSerializedTypeHandler<T> : SqlMapper.TypeHandler<T>
		where T : new() {
		/// <summary>
		/// Parse from json
		/// </summary>
		public override T Parse(object value) {
			var json = value?.ToString();
			if (string.IsNullOrEmpty(json)) {
				return new T();
			}
			return JsonConvert.DeserializeObject<T>(value?.ToString());
		}

		/// <summary>
		/// Serialize to json
		/// </summary>
		public override void SetValue(IDbDataParameter parameter, T value) {
			parameter.Value = JsonConvert.SerializeObject(value);
		}
	}
}
