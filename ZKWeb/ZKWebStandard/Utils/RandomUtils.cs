using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Random utility functions<br/>
	/// <br/>
	/// </summary>
	public static class RandomUtils {
		/// <summary>
		/// Random generator<br/>
		/// <br/>
		/// </summary>
		public static Random Generator { get; } = new Random(SystemRandomInt());

		/// <summary>
		/// Create secure random bytes in given length<br/>
		/// <br/>
		/// </summary>
		public static byte[] SystemRandomBytes(int length) {
			byte[] buffer = new byte[length];
			using (var rng = RandomNumberGenerator.Create()) {
				rng.GetBytes(buffer);
			}
			return buffer;
		}

		/// <summary>
		/// Create secure random integer<br/>
		/// <br/>
		/// </summary>
		public static int SystemRandomInt() {
			return BitConverter.ToInt32(SystemRandomBytes(4), 0);
		}

		/// <summary>
		/// Create random integer<br/>
		/// <br/>
		/// </summary>
		/// <param name="minValue">Min value, inclusive</param>
		/// <param name="maxValue">Max value, exclusive</param>
		/// <returns></returns>
		public static int RandomInt(int minValue, int maxValue) {
			return Generator.Next(minValue, maxValue);
		}

		/// <summary>
		/// Randomly select a value from collection<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="values">Values</param>
		/// <returns></returns>
		public static T RandomSelection<T>(IList<T> values) {
			if (!values.Any()) {
				return default(T);
			}
			return values[Generator.Next(0, values.Count - 1)];
		}

		/// <summary>
		/// Randonly select a enum value from enum type<br/>
		/// If enum type is empty, then return 0<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <returns></returns>
		public static TEnum RandomEnum<TEnum>()
			where TEnum : struct, IConvertible {
			var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToList();
			return RandomSelection(values);
		}

		/// <summary>
		/// Generate random string in given length<br/>
		/// <br/>
		/// </summary>
		/// <param name="length">String length</param>
		/// <param name="chars">With chars, default is a-zA-Z0-9</param>
		/// <returns></returns>
		public static string RandomString(int length,
			string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
			var buffer = new char[length];
			for (int n = 0; n < length; ++n) {
				buffer[n] = chars[Generator.Next(chars.Length)];
			}
			return new string(buffer);
		}
	}
}
