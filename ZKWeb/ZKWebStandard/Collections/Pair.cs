using System;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Pair type<br/>
	/// Why use it instead of Tuple: Tuple is class, not fast as struct for creating object<br/>
	/// Why use it instead of KeyValuePair:<br/>
	/// - KeyValuePair didn't override GetHashCode,<br/>
	/// - it will be very slow to use KeyValuePair for dictionary key<br/>
	/// 对类型<br/>
	/// 为什么用它替代Tuple: Tuple是类, 创建对象的速度比struct要慢<br/>
	/// 为什么用它替代KeyValuePair:<br/>
	/// - KeyValuePair未实现GetHashCode,<br/>
	/// - 因此在使用KeyValuePair作为词典键的时候会非常的慢<br/>
	/// </summary>
	/// <typeparam name="TFirst">First value's type</typeparam>
	/// <typeparam name="TSecond">Second value's type</typeparam>
	/// <example>
	/// <code>
	/// var pair = Pair.Create(1, "abc");
	/// var first = pair.First; // 1
	/// var second = pair.Second; // "abc"
	/// </code>
	/// </example>
	/// <seealso cref="Pair"/>
	public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>> {
		/// <summary>
		/// First value<br/>
		/// 第一个值<br/>
		/// </summary>
		public TFirst First { get; private set; }
		/// <summary>
		/// 第二个值Second value<br/>
		/// <br/>
		/// </summary>
		public TSecond Second { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="first">First value</param>
		/// <param name="second">Second value</param>
		public Pair(TFirst first, TSecond second) {
			First = first;
			Second = second;
		}

		/// <summary>
		/// Compare to the given object for equality<br/>
		/// 检查是否与参数中对象相等<br/>
		/// </summary>
		/// <param name="obj">Other object</param>
		/// <returns></returns>
		public bool Equals(Pair<TFirst, TSecond> obj) {
			return First.EqualsSupportsNull(obj.First) && Second.EqualsSupportsNull(obj.Second);
		}

		/// <summary>
		/// Compare to the given object for equality<br/>
		/// 检查是否与参数中的对象相等<br/>
		/// </summary>
		/// <param name="obj">Other object</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is Pair<TFirst, TSecond>) && Equals((Pair<TFirst, TSecond>)obj);
		}

		/// <summary>
		/// Get hash code<br/>
		/// 获取哈希值<br/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			// same with Tuple.CombineHashCodess
			var hash_1 = First?.GetHashCode() ?? 0;
			var hash_2 = Second?.GetHashCode() ?? 0;
			return (hash_1 << 5) + hash_1 ^ hash_2;
		}

		/// <summary>
		/// To string<br/>
		/// 转换到字符串<br/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return $"({First?.ToString() ?? "null"}, {Second.ToString() ?? "null"})";
		}

		/// <summary>
		/// Support deconstruction<br/>
		/// 支持解构<br/>
		/// </summary>
		/// <param name="first">First value</param>
		/// <param name="second">Second value</param>
		public void Deconstruct(out TFirst first, out TSecond second) {
			first = First;
			second = Second;
		}
	}

	/// <summary>
	/// Pair type utility functions<br/>
	/// 对类型的工具函数<br/>
	/// </summary>
	/// <seealso cref="Pair{TFirst, TSecond}"/>
	public static class Pair {
		/// <summary>
		/// Create pair<br/>
		/// 创建对实例<br/>
		/// </summary>
		/// <typeparam name="TFirst">First value's type</typeparam>
		/// <typeparam name="TSecond">Second value's type</typeparam>
		/// <param name="first">First value</param>
		/// <param name="second">Second value</param>
		/// <returns></returns>
		public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second) {
			return new Pair<TFirst, TSecond>(first, second);
		}
	}
}
