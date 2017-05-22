using System;
using System.ComponentModel;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Enum extension methods<br/>
	/// <br/>
	/// </summary>
	public static class EnumExtensions {
		/// <summary>
		/// Get enum value description<br/>
		/// If enum value contains `DescriptionAttribute` then return the description in attribute<br/>
		/// Otherwise return the default name<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="value">Enum value</param>
		/// <returns></returns>
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
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Attribute type</typeparam>
		/// <param name="value">Enum value</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Enum value)
			where T : Attribute {
			Type type = value.GetType();
			var field = type.FastGetField(Enum.GetName(type, value));
			return field.GetAttribute<T>();
		}
	}
}
