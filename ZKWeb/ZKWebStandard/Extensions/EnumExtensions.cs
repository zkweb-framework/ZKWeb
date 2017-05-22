using System;
using System.ComponentModel;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Enum extension methods<br/>
	/// 枚举值的扩展函数<br/>
	/// </summary>
	public static class EnumExtensions {
		/// <summary>
		/// Get enum value description<br/>
		/// If enum value contains `DescriptionAttribute` then return the description in attribute<br/>
		/// Otherwise return the default name<br/>
		/// 获取枚举值的描述<br/>
		/// 如果枚举值包含了`DescriptionAttribute`属性则返回属性中的描述<br/>
		/// 否则返回默认名称<br/>
		/// </summary>
		/// <param name="value">Enum value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// enum TestEnum { [Description("TestEnum_A")]A, [Description("TestEnum_B")]B, C }
		/// 
		/// var description = TestEnum.A.GetDescription(); // "TestEnum_A"
		/// description = TestEnum.B.GetDescription(); // "TestEnum_B"
		/// description = TestEnum.C.GetDescription(); // "C"
		/// description = ((TestEnum)100).GetDescription(); // "100"
		/// </code>
		/// </example>
		public static string GetDescription(this Enum value) {
			// Get enum type and name
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name == null) {
				// If it's not defined as a field then return the numeric value
				return Convert.ToInt32(value).ToString();
			}
			// Get DescriptionAttribute, return the description in attribute if exist
			var attribute = value.GetAttribute<DescriptionAttribute>();
			if (attribute != null) {
				return attribute.Description;
			}
			// Return default name
			return name;
		}

		/// <summary>
		/// Get attribute from enum value's field<br/>
		/// Return null if not found<br/>
		/// 获取枚举值的属性<br/>
		/// 找不到时返回null<br/>
		/// </summary>
		/// <typeparam name="T">Attribute type</typeparam>
		/// <param name="value">Enum value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// enum TestEnum { [Description("TestEnum_A")]A, [Description("TestEnum_B")]B, C }
		/// 
		/// var description = TestEnum.A.GetAttribute&lt;DescriptionAttribute&gt;().Description; // "TestEnum_A"
		/// description = TestEnum.B.GetAttribute&lt;DescriptionAttribute&gt;().Description; // "TestEnum_B"
		/// var attribute = TestEnum.C.GetAttribute&lt;DescriptionAttribute&gt;(); // null
		/// </code>
		/// </example>
		public static T GetAttribute<T>(this Enum value)
			where T : Attribute {
			Type type = value.GetType();
			var field = type.FastGetField(Enum.GetName(type, value));
			return field.GetAttribute<T>();
		}
	}
}
