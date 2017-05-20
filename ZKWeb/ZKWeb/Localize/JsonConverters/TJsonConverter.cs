using Newtonsoft.Json;
using System;

namespace ZKWeb.Localize.JsonConverters {
	/// <summary>
	/// Support serialize and deserialize T with json<br/>
	/// 支持json序列化和反序列化T类型<br/>
	/// </summary>
	public class TJsonConverter : JsonConverter {
		/// <summary>
		/// Determine the given type can use this conveter<br/>
		/// 岸段类型是否可以用这个转换器<br/>
		/// </summary>
		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(T));
		}

		/// <summary>
		/// Read json as object<br/>
		/// 把json转换为对象<br/>
		/// </summary>
		public override object ReadJson(JsonReader reader,
			Type objectType, object existingValue, JsonSerializer serializer) {
			return new T(reader.Value as string);
		}

		/// <summary>
		/// Write object to json<br/>
		/// 把对象转换为json<br/>
		/// </summary>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			writer.WriteValue(value?.ToString());
		}
	}
}
