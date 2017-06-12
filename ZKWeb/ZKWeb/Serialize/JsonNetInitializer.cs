using Newtonsoft.Json;
using ZKWebStandard.Extensions;

namespace ZKWeb.Serialize {
	/// <summary>
	/// Set Json.Net's default settings<br/>
	/// 修改Json.Net的默认设置<br/>
	/// </summary>
	public class JsonNetInitializer {
		/// <summary>
		/// Set Json.Net's default settings<br/>
		/// 修改Json.Net的默认设置<br/>
		/// </summary>
		internal protected virtual void Initialize() {
			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings();
				// Add custom conveters
				var converters = Application.Ioc.ResolveMany<JsonConverter>();
				converters.ForEach(c => settings.Converters.Add(c));
				// Avoid add items to exist collection, see
				// http://stackoverflow.com/questions/24835262/repeated-serialization-and-deserialization-creates-duplicate-items
				settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
				return settings;
			};
		}
	}
}
