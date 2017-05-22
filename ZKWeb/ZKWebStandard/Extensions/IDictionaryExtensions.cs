using System;
using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IDictionary extension methods<br/>
	/// <br/>
	/// </summary>
	public static class IDictionaryExtensions {
		/// <summary>
		/// Get value, return the default value if not found<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key, if it's null then the default value will be returned</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue)) {
			TValue value;
			if (key != null && dict.TryGetValue(key, out value)) {
				return value;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get value, create a new value if not found<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static TValue GetOrCreate<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValue) {
			TValue value;
			if (dict.TryGetValue(key, out value)) {
				return value;
			}
			dict[key] = (value = defaultValue());
			return value;
		}

		/// <summary>
		/// Get value and convert it to the given type<br/>
		/// Return the default value if not found<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">The type convert to</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static T GetOrDefault<T>(
			this IDictionary<string, object> dict, string key, T defaultValue = default(T)) {
			object result = dict.GetOrDefault(key);
			if (result == null) {
				return defaultValue;
			}
			return result.ConvertOrDefault(defaultValue);
		}
	}
}
