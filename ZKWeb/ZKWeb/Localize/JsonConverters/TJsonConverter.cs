using Newtonsoft.Json;
using System;

namespace ZKWeb.Localize.JsonConverters {
	/// <summary>
	/// Support serialize and deserialize T with json
	/// </summary>
	public class TJsonConverter : JsonConverter {
		/// <summary>
		/// Determine the given type can use this conveter
		/// </summary>
		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(T));
		}

		/// <summary>
		/// Read json as object
		/// </summary>
		public override object ReadJson(JsonReader reader,
			Type objectType, object existingValue, JsonSerializer serializer) {
			return new T(reader.Value as string);
		}

		/// <summary>
		/// Write object to json
		/// </summary>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			writer.WriteValue(value?.ToString());
		}
	}
}
