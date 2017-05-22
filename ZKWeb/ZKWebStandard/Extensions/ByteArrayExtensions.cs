using System;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Byte array extension methods<br/>
	/// 字节数组的扩展函数<br/>
	/// </summary>
	public static class ByteArrayExtensions {
		/// <summary>
		/// Convert byte array to hex former, in lower case<br/>
		/// 转换字节数组到Hex格式, 且英文字母是小写<br/>
		/// </summary>
		/// <param name="data">Byte array</param>
		/// <returns></returns>
		public static string ToHex(this byte[] data) {
			if (data == null) {
				return "";
			}
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}
