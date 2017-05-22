using Newtonsoft.Json;
using System;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Object extension methods<br/>
	/// <br/>
	/// </summary>
	public static class ObjectExtensions {
		/// <summary>
		/// Compare objects, won't throw exception if any object is null<br/>
		/// <br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="target">Target object</param>
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
		/// Convert object to specified type<br/>
		/// Return default value if failed<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Type convert to</typeparam>
		/// <param name="obj">Object</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>
		public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T)) {
			return (T)obj.ConvertOrDefault(typeof(T), defaultValue);
		}

		/// <summary>
		/// Convert object to specified type<br/>
		/// Return default value if failed<br/>
		/// Flow<br/>
		/// - If object is Enum and type is int, use Convert.ToInt32<br/>
		/// - If object is string and type is Enum, use Enum.Parse<br/>
		/// - Use Convert.ChangeType<br/>
		/// - If object is string, use json deserialize(obj, type)<br/>
		/// - Use json deserialize(serialize(obj), type)<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="type">Target type</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>
		public static object ConvertOrDefault(this object obj, Type type, object defaultValue) {
			// If object is null, we don't need to convert
			if (obj == null) {
				return defaultValue;
			}
			// If object can convert to type directly, we don't need to convert
			var objType = obj.GetType();
			if (type.GetTypeInfo().IsAssignableFrom(objType)) {
				return obj;
			}
			// Handle enum and use Convert
			try {
				if (objType.GetTypeInfo().IsEnum && type == typeof(int)) {
					// enum => int
					return Convert.ToInt32(obj);
				} else if (objType == typeof(string) && type.GetTypeInfo().IsEnum) {
					// string => enum
					return Enum.Parse(type, (string)obj);
				}
				return Convert.ChangeType(obj, type);
			} catch {
			}
			// Use JsonConvert, use obj as json string
			if (obj is string) {
				try { return JsonConvert.DeserializeObject(obj as string, type); } catch { }
			}
			// Use JsonConvert, serialize then deserialize
			try {
				return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), type);
			} catch {
			}
			return defaultValue;
		}

		/// <summary>
		/// Use json serializer to clone object<br/>
		/// Please sure the object can serialize and deserialize by json.net<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="obj">Object</param>
		/// <returns></returns>
		public static T CloneByJson<T>(this T obj) {
			var json = JsonConvert.SerializeObject(obj);
			var objClone = JsonConvert.DeserializeObject<T>(json);
			return objClone;
		}
	}
}
