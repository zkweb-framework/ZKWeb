using System.Text.RegularExpressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Regex utility functions<br/>
	/// 正则表达式的工具函数<br/>
	/// </summary>
	public static class RegexUtils {
		/// <summary>
		/// Expressions<br/>
		/// 正则表达式<br/>
		/// </summary>
		public static class Expressions {
			/// <summary>
			/// Email<br/>
			/// 邮箱<br/>
			/// </summary>
			public const string Email = @"^[\w-]+@[\w-]+(\.[\w-]+)+$";
			/// <summary>
			/// China mobile<br/>
			/// 中国的手机号码<br/>
			/// </summary>
			public const string ChinaMobile = "^1[3456789][0-9]{9}$";
			/// <summary>
			/// Digits<br/>
			/// 整数<br/>
			/// </summary>
			public const string Digits = @"^-?[\d]+$";
			/// <summary>
			/// Decimal<br/>
			/// 整数和小数<br/>
			/// </summary>
			public const string Decimal = @"^-?[\d]+(\.[\d]+)?$";
			/// <summary>
			/// Tel<br/>
			/// 电话号码<br/>
			/// </summary>
			public const string Tel = @"^\+?[\d\s-]+$";
		}

		/// <summary>
		/// Regex objects<br/>
		/// 正则对象<br/>
		/// </summary>
		public static class Validators {
			/// <summary>
			/// Email<br/>
			/// 邮箱<br/>
			/// </summary>
			public static Regex Email { get; } = new Regex(Expressions.Email);
			/// <summary>
			/// China mobile<br/>
			/// 中国的手机号码<br/>
			/// </summary>
			public static Regex ChinaMobile { get; } = new Regex(Expressions.ChinaMobile);
			/// <summary>
			/// Digits<br/>
			/// 整数<br/>
			/// </summary>
			public static Regex Digits { get; } = new Regex(Expressions.Digits);
			/// <summary>
			/// Decimal<br/>
			/// 整数和小数<br/>
			/// </summary>
			public static Regex Decimal { get; } = new Regex(Expressions.Decimal);
			/// <summary>
			/// Tel<br/>
			/// 电话<br/>
			/// </summary>
			public static Regex Tel { get; } = new Regex(Expressions.Tel);
		}
	}
}
