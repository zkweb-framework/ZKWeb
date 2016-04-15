using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 随机工具类
	/// </summary>
	public static class RandomUtils {
		/// <summary>
		/// 随机数生成器
		/// </summary>
		public static Random Generator { get; } = new Random(SystemRandomInt());

		/// <summary>
		/// 使用RNGCryptoServiceProvider生成真正随机的二进制数据
		/// </summary>
		public static byte[] SystemRandomBytes(int length) {
			byte[] buffer = new byte[length];
			using (var rng = new RNGCryptoServiceProvider()) {
				rng.GetBytes(buffer);
			}
			return buffer;
		}

		/// <summary>
		/// 使用RNGCryptoServiceProvider生成真正随机的整数
		/// </summary>
		public static int SystemRandomInt() {
			return BitConverter.ToInt32(SystemRandomBytes(4), 0);
		}

		/// <summary>
		/// 生成随机数值
		/// </summary>
		/// <param name="minValue">最小值</param>
		/// <param name="maxValue">最大值</param>
		/// <returns></returns>
		public static int RandomInt(int minValue, int maxValue) {
			return Generator.Next(minValue, maxValue);
		}

		/// <summary>
		/// 从列表中随机选择一个元素
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="values">元素列表</param>
		/// <returns></returns>
		public static T RandomSelection<T>(IList<T> values) {
			if (!values.Any()) {
				return default(T);
			}
			return values[Generator.Next(0, values.Count - 1)];
		}

		/// <summary>
		/// 生成随机枚举值
		/// 除非是空枚举，否则不会选择没有定义的值
		/// </summary>
		/// <typeparam name="TEnum">枚举类型</typeparam>
		/// <returns></returns>
		public static TEnum RandomEnum<TEnum>()
			where TEnum : struct, IConvertible {
			var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToList();
			return RandomSelection(values);
		}

		/// <summary>
		/// 生成随机字符串
		/// </summary>
		/// <param name="length">字符串长度</param>
		/// <param name="chars">包含的字符，默认是a-zA-Z0-9</param>
		/// <returns></returns>
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
