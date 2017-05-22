using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert utility functions<br/>
	/// <br/>
	/// </summary>
	public static class Assert {
		/// <summary>
		/// Test if condition is true<br/>
		/// <br/>
		/// </summary>
		public static void IsTrue(bool condition,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			if (!condition) {
				throw new AssertException(
					$"assert is true failed: {filePath}:{lineNumber} {memberName}");
			}
		}

		/// <summary>
		/// Test if condition is true<br/>
		/// If failed include `obj`'s contents to exception message<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public static void IsTrueWith(bool condition, object obj,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			if (!condition) {
				var objJson = JsonConvert.SerializeObject(obj);
				throw new AssertException(
					$"assert is true failed: obj is {objJson}, {filePath}:{lineNumber} {memberName}");
			}
		}

		/// <summary>
		/// Test if a and b are equals<br/>
		/// <br/>
		/// </summary>
		public static void Equals<T>(T a, T b,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			if (!a.EqualsSupportsNull(b)) {
				throw new AssertException(
					$"assert equals failed: {a} != {b}, {filePath}:{lineNumber} {memberName}");
			}
		}

		/// <summary>
		/// Test if a and b are equals<br/>
		/// Use to avoid calling object.Equals<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public static void Equals(object a, object b,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			Equals<object>(a, b, memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Test if action throws specified type's exception<br/>
		/// <br/>
		/// </summary>
		public static void Throws<TException>(Action action,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) where TException : Exception {
			try {
				action();
			} catch (TException) {
				return;
			} catch (Exception ex) {
				throw new AssertException(
					$"assert throws failed: ex is {ex}, {filePath}:{lineNumber} {memberName}");
			}
			throw new AssertException(
				$"assert throws failed: no exception throwed, {filePath}:{lineNumber} {memberName}");
		}

		/// <summary>
		/// Make this test passed<br/>
		/// <br/>
		/// </summary>
		public static void Passed() {
			throw new AssertPassedException();
		}

		/// <summary>
		/// Make this test skipped<br/>
		/// <br/>
		/// </summary>
		public static void Skipped(string reason) {
			throw new AssertSkipedException(reason);
		}
	}
}
