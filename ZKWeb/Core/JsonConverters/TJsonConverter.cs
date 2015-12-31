using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Core.JsonConverters {
	/// <summary>
	/// 支持序列化和反序列化类型T
	/// </summary>
	public class TJsonConverter : JsonConverter {
		/// <summary>
		/// 判断是否可以序列化
		/// </summary>
		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(T));
		}

		/// <summary>
		/// json到对象
		/// </summary>
		public override object ReadJson(JsonReader reader,
			Type objectType, object existingValue, JsonSerializer serializer) {
			return new T(reader.ReadAsString());
		}

		/// <summary>
		/// 对象到json
		/// </summary>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			writer.WriteValue(value?.ToString());
		}
	}
}
