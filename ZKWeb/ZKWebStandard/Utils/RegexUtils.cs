using System.Text.RegularExpressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Regex utility functions
	/// </summary>
	public static class RegexUtils {
		/// <summary>
		/// Expressions
		/// </summary>
		public static class Expressions {
			/// <summary>
			/// Email
			/// </summary>
			public const string Email = @"^[\w-]+@[\w-]+(\.[\w-]+)+$";
			/// <summary>
			/// China mobile
			/// </summary>
			public const string ChinaMobile = "^1[3456789][0-9]{9}$";
			/// <summary>
			/// Digits
			/// </summary>
			public const string Digits = @"^-?[\d]+$";
			/// <summary>
			/// Decimal
			/// </summary>
			public const string Decimal = @"^-?[\d]+(\.[\d]+)?$";
			/// <summary>
			/// Tel
			/// </summary>
			public const string Tel = @"^\+?[\d\s-]+$";
		}

		/// <summary>
		/// Regex objects
		/// </summary>
		public static class Validators {
			/// <summary>
			/// Email
			/// </summary>
			public static Regex Email { get; } = new Regex(Expressions.Email);
			/// <summary>
			/// China mobile
			/// </summary>
			public static Regex ChinaMobile { get; } = new Regex(Expressions.ChinaMobile);
			/// <summary>
			/// Digits
			/// </summary>
			public static Regex Digits { get; } = new Regex(Expressions.Digits);
			/// <summary>
			/// Decimal
			/// </summary>
			public static Regex Decimal { get; } = new Regex(Expressions.Decimal);
			/// <summary>
			/// Tel
			/// </summary>
			public static Regex Tel { get; } = new Regex(Expressions.Tel);
		}
	}
}
