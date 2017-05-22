using System;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Pair type<br/>
	/// Why use it instead of Tuple: Tuple is class, not fast as struct for creating object<br/>
	/// Why use it instead of KeyValuePair:<br/>
	/// - KeyValuePair didn't override GetHashCode,<br/>
	/// - it will be very slow to use KeyValuePair for dictionary key<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="TFirst">First value's type</typeparam>
	/// <typeparam name="TSecond">Second value's type</typeparam>
	public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>> {
		/// <summary>
		/// First value<br/>
		/// <br/>
		/// </summary>
		public TFirst First { get; private set; }
		/// <summary>
		/// Second value<br/>
		/// <br/>
		/// </summary>
		public TSecond Second { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="first">First value</param>
		/// <param name="second">Second value</param>
		public Pair(TFirst first, TSecond second) {
			First = first;
			Second = second;
		}

		/// <summary>
		/// Compare to the given object for equality<br/>
		/// <br/>
		/// </summary>
		/// <param name="obj">Other object</param>
		/// <returns></returns>
		public bool Equals(Pair<TFirst, TSecond> obj) {
			return First.EqualsSupportsNull(obj.First) && Second.EqualsSupportsNull(obj.Second);
		}

		/// <summary>
		/// Compare to the given object for equality<br/>
		/// <br/>
		/// </summary>
		/// <param name="obj">Other object</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is Pair<TFirst, TSecond>) && Equals((Pair<TFirst, TSecond>)obj);
		}

		/// <summary>
		/// Get hash code<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			// same with Tuple.CombineHashCodess
			var hash_1 = First?.GetHashCode() ?? 0;
			var hash_2 = Second?.GetHashCode() ?? 0;
			return (hash_1 << 5) + hash_1 ^ hash_2;
		}
	}

	/// <summary>
	/// Pair type utility functions<br/>
	/// <br/>
	/// </summary>
	public static class Pair {
		/// <summary>
		/// Create pair<br/>
		/// <br/>
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
