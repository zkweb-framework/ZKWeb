using System;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Byte array extension methods<br/>
	/// <br/>
	/// </summary>
	public static class ByteArrayExtensions {
		/// <summary>
		/// Convert byte array to hex former, in lower case<br/>
		/// <br/>
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
