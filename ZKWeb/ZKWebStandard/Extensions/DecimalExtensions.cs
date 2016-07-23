namespace ZKWebStandard.Extensions {
	/// <summary>
	/// decimal使用的扩展函数
	/// </summary>
	public static class DecimalExtensions {
		/// <summary>
		/// 去除decimal后多余的0
		/// 传入12.3000时会返回12.3
		/// 来源: http://stackoverflow.com/questions/4298719/parse-decimal-and-filter-extra-0-on-the-right
		/// </summary>
		public static decimal Normalize(this decimal value) {
			return value / 1.000000000000000000000000000000000m;
		}
	}
}
