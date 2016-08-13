using System;
using System.ComponentModel;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Enum extension methods
	/// </summary>
	public static class EnumExtensions {
		/// <summary>
		/// Get enum value description
		/// If enum value contains `DescriptionAttribute` then return the description in attribute
		/// Otherwise return the default name
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
		/// Get attribute from enum value's field
		/// Return null if not found
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
