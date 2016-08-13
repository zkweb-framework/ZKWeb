namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Decimal extension methods
	/// </summary>
	public static class DecimalExtensions {
		/// <summary>
		/// Remove excess 0s after decimal
		/// Eg: giving 12.3000 will return 12.3
		/// See: http://stackoverflow.com/questions/4298719/parse-decimal-and-filter-extra-0-on-the-right
		/// </summary>
		public static decimal Normalize(this decimal value) {
			return value / 1.000000000000000000000000000000000m;
		}
	}
}
