using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// byte[]的扩展函数
	/// </summary>
	public static class ByteArrayExtensions {
		/// <summary>
		/// 把byte[]转换到hex格式，返回小写
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string ToHex(this byte[] data) {
			if (data == null) {
				return "";
			}
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}
