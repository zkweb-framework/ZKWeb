using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.UnitTest {
	/// <summary>
	/// 断言类
	/// </summary>
	public static class Assert {
		/// <summary>
		/// 测试条件是否成立
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
		/// 测试条件是否成立
		/// 不成立时包含指定的对象的信息到例外中
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
		/// 测试对象是否相等
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
		/// 测试对象是否相等
		/// 对象版本，提供这个函数用于防止object.Equals被调用
		/// </summary>
		public static void Equals(object a, object b,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			Equals<object>(a, b, memberName, filePath, lineNumber);
		}

		/// <summary>
		/// 测试指定的函数抛出指定的例外
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
		/// 把当前测试作为通过处理
		/// </summary>
		public static void Passed() {
			throw new AssertPassedException();
		}

		/// <summary>
		/// 把当前测试作为跳过处理
		/// </summary>
		public static void Skipped(string reason) {
			throw new AssertSkipedException(reason);
		}
	}
}
