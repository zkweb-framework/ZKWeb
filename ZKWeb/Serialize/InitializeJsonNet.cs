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
				// 添加自定义的转换器
				var converters = Application.Ioc.ResolveMany<JsonConverter>();
				converters.ForEach(c => settings.Converters.Add(c));
				// 防止反序列化时使用原来的对象，导致元素重复
				// http://stackoverflow.com/questions/24835262/repeated-serialization-and-deserialization-creates-duplicate-items
				settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
				return settings;
			};
		}
	}
}
