using System;
using System.ComponentModel;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// 枚举类型的扩展函数
	/// </summary>
	public static class EnumExtensions {
		/// <summary>
		/// 获取枚举值的描述
		/// 有指定Description属性时返回该属性的对应名称，否则返回字段本身的名称
		/// </summary>
		/// <param name="value">枚举值</param>
		/// <returns></returns>
		public static string GetDescription(this Enum value) {
			// 获取枚举值类型和名称
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name == null) {
				// 值不在类型中时返回数字
				return Convert.ToInt32(value).ToString();
			}
			// 获取Description属性
			var attribute = value.GetAttribute<DescriptionAttribute>();
			if (attribute != null) {
				return attribute.Description;
			}
			// 返回默认名称
			return name;
		}

		/// <summary>
		/// 获取枚举值的属性
		/// 没有对应属性时返回null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">枚举值</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Enum value)
			where T : Attribute {
			Type type = value.GetType();
			var field = type.FastGetField(Enum.GetName(type, value));
			return Attribute.GetCustomAttribute(field, typeof(T)) as T;
		}
	}
}
