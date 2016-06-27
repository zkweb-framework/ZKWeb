using Newtonsoft.Json;
using System;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// 对象的扩展函数
	/// </summary>
	public static class ObjectExtensions {
		/// <summary>
		/// 对比对象是否相等
		/// 对象等于null也可以进行比较
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="target">目标对象</param>
		/// <returns></returns>
		public static bool EqualsSupportsNull(this object obj, object target) {
			if (obj == null && target == null) {
				return true;
			} else if (obj == null && target != null) {
				return false;
			} else if (obj != null && target == null) {
				return false;
			}
			return object.ReferenceEquals(obj, target) || obj.Equals(target);
		}

		/// <summary>
		/// 转换对象到指定类型，失败时返回默认值
		/// </summary>
		/// <typeparam name="T">需要转换到的类型</typeparam>
		/// <param name="obj">转换的对象</param>
		/// <param name="default_value">默认值</param>
		/// <returns></returns>
		public static T ConvertOrDefault<T>(this object obj, T default_value = default(T)) {
			return (T)obj.ConvertOrDefault(typeof(T), default_value);
		}

		/// <summary>
		/// 转换对象到指定类型，失败时返回默认值
		/// 转换过程
		///		类型是枚举类型时先把值转换到int再转换到枚举类型
		///		使用Convert.ChangeType转换
		///		使用JsonConvert转换
		/// </summary>
		/// <param name="obj">转换的对象</param>
		/// <param name="type">需要转换到的类型</param>
		/// <param name="default_value">默认值</param>
		/// <returns></returns>
		public static object ConvertOrDefault(this object obj, Type type, object default_value) {
			// 对象是null时直接返回默认值
			if (obj == null) {
				return default_value;
			}
			// 类型相同时直接返回，不需要转换
			var objType = obj.GetType();
			if (type.GetTypeInfo().IsAssignableFrom(objType)) {
				return obj;
			}
			// 使用Convert转换
			try {
				if (objType.GetTypeInfo().IsEnum && type == typeof(int)) {
					// enum => int
					return Convert.ToInt32(obj);
				} else if (objType == typeof(string) && type.GetTypeInfo().IsEnum) {
					// string => enum, 下面json也能转换但这里转换性能更快
					return Enum.Parse(type, (string)obj);
				}
				return Convert.ChangeType(obj, type);
			} catch {
			}
			// 使用JsonConvert转换
			if (obj is string) {
				try { return JsonConvert.DeserializeObject(obj as string, type); } catch { }
			}
			try {
				return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), type);
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
