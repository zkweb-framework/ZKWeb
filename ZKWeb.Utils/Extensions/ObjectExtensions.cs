using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 对象的扩展函数
	/// </summary>
	public static class ObjectExtensions {
		/// <summary>
		/// 转换对象到指定类型，失败时返回默认值
		/// 转换过程
		///		类型是枚举类型时先把值转换到int再转换到枚举类型
		///		使用Convert.ChangeType转换
		///		使用JsonConvert转换
		/// </summary>
		/// <typeparam name="T">x需要转换到的类型</typeparam>
		/// <param name="obj">转换的对象</param>
		/// <param name="default_value">默认值</param>
		/// <returns></returns>
		public static T ConvertOrDefault<T>(this object obj, T default_value = default(T)) {
			// 使用Convert转换
			Type type = typeof(T);
			try {
				if (type.IsEnum) {
					return (T)(object)Convert.ToInt32(obj);
				}
				return (T)Convert.ChangeType(obj, type);
			} catch {
			}
			// 使用JsonConvert转换
			try {
				return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
			} catch {
			}
			return default_value;
		}

		/// <summary>
		/// 通过Json序列化和反序列化克隆对象
		/// 调用此函数前必须确认对象可以通过Json序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T CloneByJson<T>(this T obj) {
			var json = JsonConvert.SerializeObject(obj);
			var objClone = JsonConvert.DeserializeObject<T>(json);
			return objClone;
		}
	}
}
