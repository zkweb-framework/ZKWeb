using System;
using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IDictionary extension methods<br/>
	/// 词典的扩展函数<br/>
	/// </summary>
	public static class IDictionaryExtensions {
		/// <summary>
		/// Get value, return the default value if not found<br/>
		/// 获取值, 找不到时返回默认值<br/>
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key, if it's null then the default value will be returned</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var dict = new Dictionary&lt;string, string&gt;() { { "exist", "1" } };
		/// var value = dict.GetOrDefault("exist"); // "1"
		/// value = dict.GetOrDefault("notexist"); // null
		/// value = dict.GetOrDefault("notexist", "default"); // "default"
		/// value = dict.GetOrDefault(null, "default"); // "default"
		/// value = dict.GetOrDefault("", "default"); // "default"
		/// </code>
		/// </example>
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue)) {
			TValue value;
			if (dict.Count > 0 && key != null && dict.TryGetValue(key, out value)) {
				return value;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get value, create a new value if not found<br/>
		/// 获取值, 不存在时创建新值<br/>
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var dict = new Dictionary&lt;string, string&gt;() { { "exist", "1" } };
		/// var value = dict.GetOrCreate("exist", () =&gt; "create"); // "1"
		/// value = dict.GetOrCreate("notexist", () =&gt; "create"); // "create"
		/// value = dict["notexist"]; // "create"
		/// </code>
		/// </example>
		public static TValue GetOrCreate<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValue) {
			TValue value;
			if (dict.Count > 0 && dict.TryGetValue(key, out value)) {
				return value;
			}
			dict[key] = (value = defaultValue());
			return value;
		}

		/// <summary>
		/// Get value and convert it to the given type<br/>
		/// Return the default value if not found<br/>
		/// 获取值并转换到指定类型<br/>
		/// 找不到时返回默认值<br/>
		/// </summary>
		/// <typeparam name="T">The type convert to</typeparam>
		/// <param name="dict">The dictionary</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// enum TestEnum { Zero = 0, One = 1 }
		/// 
		/// var dict = new Dictionary&lt;string, object&gt;() { { "exist", 1 } };
		/// var value = dict.GetOrDefault&lt;int&gt;("exist"); // 1
		/// value = dict.GetOrDefault&lt;int&gt;("notexist"); // 0
		/// value = dict.GetOrDefault("notexist", 100); // 100
		/// value = dict.GetOrDefault&lt;TestEnum&gt;("exist"); // TestEnum.One
		/// value = dict.GetOrDefault&lt;TestEnum&gt;("notexist"); // TestEnum.Zero
		/// </code>
		/// </example>
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
