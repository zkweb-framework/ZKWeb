using System.Text.RegularExpressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// 正则工具类
	/// </summary>
	public static class RegexUtils {
		/// <summary>
		/// 表达式
		/// </summary>
		public static class Expressions {
			/// <summary>
			/// 邮箱
			/// </summary>
			public const string Email = @"^[\w-]+@[\w-]+\.[\w-]+$";
			/// <summary>
			/// 中国手机号
			/// </summary>
			public const string ChinaMobile = "^1[3456789][0-9]{9}$";
			/// <summary>
			/// 整数
			/// </summary>
			public const string Digits = @"^-?[\d]+$";
			/// <summary>
			/// 整数和小数
			/// </summary>
			public const string Decimal = @"^-?[\d]+(\.[\d]+)?$";
			/// <summary>
			/// 电话号码
			/// </summary>
			public const string Tel = @"^\+?[\d\s-]+$";
		}

		/// <summary>
		/// 表达式对象
		/// </summary>
		public static class Validators {
			/// <summary>
			/// 邮箱
			/// </summary>
			public static Regex Email { get; } = new Regex(Expressions.Email);
			/// <summary>
			/// 中国手机号
			/// </summary>
			public static Regex ChinaMobile { get; } = new Regex(Expressions.ChinaMobile);
			/// <summary>
			/// 整数
			/// </summary>
			public static Regex Digits { get; } = new Regex(Expressions.Digits);
			/// <summary>
			/// 整数和小数
			/// </summary>
			public static Regex Decimal { get; } = new Regex(Expressions.Decimal);
			/// <summary>
			/// 电话号码
			/// </summary>
			public static Regex Tel { get; } = new Regex(Expressions.Tel);
		}
	}
}
