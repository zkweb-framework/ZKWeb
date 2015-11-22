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
