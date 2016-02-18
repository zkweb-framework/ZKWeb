using DryIoc;
using DryIocAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Serialize {
	/// <summary>
	/// 设置Json.Net的默认选项
	/// </summary>
	public class InitializeJsonNet {
		/// <summary>
		/// 初始化
		/// </summary>
		public InitializeJsonNet() {
			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings();
				var converters = Application.Ioc.ResolveMany<JsonConverter>();
				converters.ForEach(c => settings.Converters.Add(c));
				return settings;
			};
		}
	}
}
