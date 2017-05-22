using System.Text.RegularExpressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Regex utility functions<br/>
	/// <br/>
	/// </summary>
	public static class RegexUtils {
		/// <summary>
		/// Expressions<br/>
		/// <br/>
		/// </summary>
		public static class Expressions {
			/// <summary>
			/// Email<br/>
			/// <br/>
			/// </summary>
			public const string Email = @"^[\w-]+@[\w-]+(\.[\w-]+)+$";
			/// <summary>
			/// China mobile<br/>
			/// <br/>
			/// </summary>
			public const string ChinaMobile = "^1[3456789][0-9]{9}$";
			/// <summary>
			/// Digits<br/>
			/// <br/>
			/// </summary>
			public const string Digits = @"^-?[\d]+$";
			/// <summary>
			/// Decimal<br/>
			/// <br/>
			/// </summary>
			public const string Decimal = @"^-?[\d]+(\.[\d]+)?$";
			/// <summary>
			/// Tel<br/>
			/// <br/>
			/// </summary>
			public const string Tel = @"^\+?[\d\s-]+$";
		}

		/// <summary>
		/// Regex objects<br/>
		/// <br/>
		/// </summary>
		public static class Validators {
			/// <summary>
			/// Email<br/>
			/// <br/>
			/// </summary>
			public static Regex Email { get; } = new Regex(Expressions.Email);
			/// <summary>
			/// China mobile<br/>
			/// <br/>
			/// </summary>
			public static Regex ChinaMobile { get; } = new Regex(Expressions.ChinaMobile);
			/// <summary>
			/// Digits<br/>
			/// <br/>
			/// </summary>
			public static Regex Digits { get; } = new Regex(Expressions.Digits);
			/// <summary>
			/// Decimal<br/>
			/// <br/>
			/// </summary>
			public static Regex Decimal { get; } = new Regex(Expressions.Decimal);
			/// <summary>
			/// Tel<br/>
			/// <br/>
			/// </summary>
			public static Regex Tel { get; } = new Regex(Expressions.Tel);
		}
	}
}
