using Newtonsoft.Json;
using System;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Object extension methods<br/>
	/// 对象的扩展函数<br/>
	/// </summary>
	public static class ObjectExtensions {
		/// <summary>
		/// Compare objects, won't throw exception if any object is null<br/>
		/// 比较对象, 对象为null时不会抛出例外<br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="target">Target object</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// ((string)null).EqualsSupportsNull("") == false
		/// "".EqualsSupportsNull(null) == false
		///	((string)null).EqualsSupportsNull(null) == true
		///	"abc".EqualsSupportsNull("abc") == true
		/// </code>
		/// </example>
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
		/// 转换对象到指定的类型<br/>
		/// 失败时返回默认值<br/>
		/// </summary>
		/// <typeparam name="T">Type convert to</typeparam>
		/// <param name="obj">Object</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// "1".ConvertOrDefault&lt;int&gt;() == 1
		/// (1).ConvertOrDefault&lt;int&gt;() == 1
		///	"abc".ConvertOrDefault&lt;int?&gt;() == null
		///	"abc".ConvertOrDefault&lt;int?&gt;(100) == 100
		///	"1.0".ConvertOrDefault&lt;decimal?&gt;() == 1.0M
		///	"1".ConvertOrDefault&lt;TestEnum?&gt;() == TestEnum.One
		///	(1).ConvertOrDefault&lt;TestEnum?&gt;() == TestEnum.One
		///	"One".ConvertOrDefault&lt;TestEnum&gt;() == TestEnum.One
		///	"One".ConvertOrDefault&lt;TestEnum?&gt;() == TestEnum.One
		///	TestEnum.One.ConvertOrDefault&lt;int&gt;() == 1
		///	TestEnum.One.ConvertOrDefault&lt;string&gt;() == "One"
		///	new List&lt;int&gt;().ConvertOrDefault&lt;int?&gt;() == null
		///	(100).ConvertOrDefault&lt;string&gt;() == "100"
		///	"test".ConvertOrDefault&lt;string&gt;() == "test"
		///	var lst = "[1]".ConvertOrDefault&lt;List&lt;int&gt;&gt;();
		/// lst.SequenceEqual(new[] { 1 })
		/// </code>
		/// </example>
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
		/// 转换对象到指定的类型<br/>
		/// 失败时返回默认值<br/>
		/// 流程<br/>
		/// - 如果对象是枚举并且类型是int, 使用Convert.ToInt32<br/>
		/// - 如果对象是字符串且类型是枚举, 使用Enum.Parse<br/>
		/// - 使用Convert.ChangeType<br/>
		/// - 如果对象是字符串, 使用json反序列化<br/>
		/// - 使用json反序列化对象的json序列化<br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="type">Target type</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// "1".ConvertOrDefault(typeof(int), 0) == 1
		/// (1).ConvertOrDefault(typeof(int), 0) == 1
		///	"abc".ConvertOrDefault(typeof(int?), null) == null
		///	"abc".ConvertOrDefault(typeof(int?), 100) == 100
		///	"1.0".ConvertOrDefault(typeof(decimal?), null) == 1.0M
		///	"1".ConvertOrDefault(typeof(TestEnum?), null) == TestEnum.One
		///	(1).ConvertOrDefault(typeof(TestEnum?), null) == TestEnum.One
		///	"One".ConvertOrDefault(typeof(TestEnum), null) == TestEnum.One
		///	"One".ConvertOrDefault(typeof(TestEnum?), null) == TestEnum.One
		///	TestEnum.One.ConvertOrDefault(typeof(int), null) == 1
		///	TestEnum.One.ConvertOrDefault(typeof(string), null) == "One"
		///	new List&lt;int&gt;().ConvertOrDefault(typeof(int?), null) == null
		///	(100).ConvertOrDefault(typeof(string), null) == "100"
		///	"test".ConvertOrDefault(typeof(string), null) == "test"
		///	var lst = "[1]".ConvertOrDefault(typeof(List&lt;int&gt;), null) as List&lt;int&gt;;
		/// lst.SequenceEqual(new[] { 1 })
		/// </code>
		/// </example>
		public static object ConvertOrDefault(this object obj, Type type, object defaultValue) {
			// If object is null, we don't need to convert
			if (obj == null) {
				return defaultValue;
			}
			// If object can convert to type directly, we don't need to convert
			var objType = obj.GetType();
			if (type.IsAssignableFrom(objType)) {
				return obj;
			}
			// Handle enum and use Convert
			try {
				if (objType.IsEnum && type == typeof(int)) {
					// enum => int
					return Convert.ToInt32(obj);
				} else if (objType == typeof(string) && type.IsEnum) {
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
		/// 使用json序列化来克隆对象<br/>
		/// 请确保对象可以通过json.net序列化和反序列化<br/>
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="obj">Object</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// class TestData {
		///		public long A { get; set; }
		///		public string B { get; set; }
		///		public bool C;
		/// }
		/// 
		/// var data = new TestData() { A = 100, B = "TestString", C = true };
		/// var dataClone = data.CloneByJson();
		/// // !object.ReferenceEquals(data, dataClone)
		/// </code>
		/// </example>
		public static T CloneByJson<T>(this T obj) {
			var json = JsonConvert.SerializeObject(obj);
			var objClone = JsonConvert.DeserializeObject<T>(json);
			return objClone;
		}
	}
}
