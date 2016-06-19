using System;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// 字节数组的扩展函数
	/// </summary>
	public static class ByteArrayExtensions {
		/// <summary>
		/// 把字节数组转换到hex格式，返回小写
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <returns></returns>
		public static string ToHex(this byte[] data) {
			if (data == null) {
				return "";
			}
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}
