using System;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// 对类型
	/// 代替Tuple的原因: Tuple是class，创建时消耗较大
	/// 代替KeyValuePair的原因: KeyValuePair没有重载GetHashCode，作为词典的键会导致频繁冲突
	/// 校验值的生成:
	/// - 这个函数生成校验值时使用hash(first) ^ hash(second)
	/// - 如果两个值相同可能会导致一直生成相同的校验，使用时应注意避免这种情况
	/// </summary>
	/// <typeparam name="TFirst">第一个值的类型</typeparam>
	/// <typeparam name="TSecond">第二个值的类型</typeparam>
	public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>> {
		/// <summary>
		/// 第一个值
		/// </summary>
		public TFirst First { get; private set; }
		/// <summary>
		/// 第二个值
		/// </summary>
		public TSecond Second { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="first">第一个值</param>
		/// <param name="second">第二个值</param>
		public Pair(TFirst first, TSecond second) {
			First = first;
			Second = second;
		}

		/// <summary>
		/// 比较是否相等
		/// </summary>
		/// <param name="obj">比较的对象</param>
		/// <returns></returns>
		public bool Equals(Pair<TFirst, TSecond> obj) {
			return First.EqualsSupportsNull(obj.First) && Second.EqualsSupportsNull(obj.Second);
		}

		/// <summary>
		/// 比较是否相等
		/// </summary>
		/// <param name="obj">比较的对象</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is Pair<TFirst, TSecond>) && Equals((Pair<TFirst, TSecond>)obj);
		}

		/// <summary>
		/// 获取校验值
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			return (First?.GetHashCode() ?? 0) ^ (Second?.GetHashCode() ?? 0);
		}
	}

	/// <summary>
	/// 对类型的静态函数
	/// </summary>
	public static class Pair {
		/// <summary>
		/// 创建对
		/// </summary>
		/// <typeparam name="TFirst">第一个值的类型</typeparam>
		/// <typeparam name="TSecond">第二个值的类型</typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second) {
			return new Pair<TFirst, TSecond>(first, second);
		}
	}
}
