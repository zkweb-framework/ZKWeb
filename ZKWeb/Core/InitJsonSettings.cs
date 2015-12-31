using DryIocAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Core.JsonConverters;

namespace ZKWeb.Core {
	/// <summary>
	/// 设置Json.Net的默认选项
	/// </summary>
	public class InitJsonSettings {
		/// <summary>
		/// 初始化
		/// </summary>
		public InitJsonSettings() {
			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new TJsonConverter());
				return settings;
			};
		}
	}
}
