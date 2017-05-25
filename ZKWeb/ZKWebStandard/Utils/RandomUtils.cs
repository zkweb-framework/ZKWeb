using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Random utility functions<br/>
	/// 随机的工具函数<br/>
	/// </summary>
	public static class RandomUtils {
		/// <summary>
		/// Random generator<br/>
		/// 全局使用的随机数生成器<br/>
		/// </summary>
		public static Random Generator { get; } = new Random(SystemRandomInt());

		/// <summary>
		/// Create secure random bytes in given length<br/>
		/// 获取安全的指定长度的随机内容<br/>
		/// </summary>
		/// <example>
		/// <code language="cs">
		/// var bytes = RandomUtils.SystemRandomBytes(20);
		/// </code>
		/// </example>
		public static byte[] SystemRandomBytes(int length) {
			byte[] buffer = new byte[length];
			using (var rng = RandomNumberGenerator.Create()) {
				rng.GetBytes(buffer);
			}
			return buffer;
		}

		/// <summary>
		/// Create secure random integer<br/>
		/// 获取安全的随机数值<br/>
		/// </summary>
		/// <example>
		/// <code language="cs">
		/// var value = RandomUtils.SystemRandomInt();
		/// </code>
		/// </example>
		public static int SystemRandomInt() {
			return BitConverter.ToInt32(SystemRandomBytes(4), 0);
		}

		/// <summary>
		/// Create random integer<br/>
		/// 获取随机数值<br/>
		/// </summary>
		/// <param name="minValue">Min value, inclusive</param>
		/// <param name="maxValue">Max value, exclusive</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var value = RandomUtils.RandomInt(0, 100);
		/// </code>
		/// </example>
		public static int RandomInt(int minValue, int maxValue) {
			return Generator.Next(minValue, maxValue);
		}

		/// <summary>
		/// Randomly select a value from collection<br/>
		/// 从集合获取随机的元素<br/>
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="values">Values</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var selection = RandomUtils.RandomSelection(options);
		/// </code>
		/// </example>
		public static T RandomSelection<T>(IList<T> values) {
			if (!values.Any()) {
				return default(T);
			}
			return values[Generator.Next(0, values.Count - 1)];
		}

		/// <summary>
		/// Randonly select a enum value from enum type<br/>
		/// If enum type is empty, then return 0<br/>
		/// 从枚举获取随机的值<br/>
		/// 如果枚举不包含任何值则返回0<br/>
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var value = RandomUtils.RandomEnum&lt;TestEnum&gt;();
		/// </code>
		/// </example>
		public static TEnum RandomEnum<TEnum>()
			where TEnum : struct, IConvertible {
			var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToList();
			return RandomSelection(values);
		}

		/// <summary>
		/// Generate random string in given length<br/>
		/// 生成指定长度的随机字符串<br/>
		/// </summary>
		/// <param name="length">String length</param>
		/// <param name="chars">With chars, default is a-zA-Z0-9</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var str = RandomUtils.RandomString(20);
		/// </code>
		/// </example>
		public static string RandomString(int length,
			string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
			var buffer = new char[length];
			for (int n = 0; n < length; ++n) {
				buffer[n] = chars[Generator.Next(chars.Length)];
			}
			return new string(buffer);
		}
	}
}
