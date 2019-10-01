using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Testing
{
    /// <summary>
    /// Assert utility functions<br/>
    /// 断言工具函数<br/>
    /// </summary>
    /// <seealso cref="TestRunner"/>
    /// <example>
    /// <code language="cs">
    /// [Tests]
    /// class ExampleTest {
    /// 	public void MethodA() {
    /// 		Assert.IsTrue(1 == 1);
    /// 		Assert.IsTrueWith(1 == 1, "if failed this item will be outputed");
    /// 		Assert.Equals(true, true);
    /// 		Assert.Throws&lt;ArgumentException&gt;(() =&gt; { throw new ArgumentException(); });
    /// 	}
    /// }
    /// </code>
    /// </example>
    public static class Assert
    {
        /// <summary>
        /// Test if condition is true<br/>
        /// 测试条件是否为真<br/>
        /// </summary>
        public static void IsTrue(
            bool condition,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!condition)
            {
                throw new AssertException(string.Format(
                    "assert is true failed\r\n  {0}:{1} {2}",
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if condition is true<br/>
        /// If failed include `obj` as contents of exception message<br/>
        /// 测试条件是否为真<br/>
        /// 如果失败会包含obj的内容到例外消息<br/>
        /// </summary>
        public static void IsTrueWith(
            bool condition,
            object obj,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!condition)
            {
                var objJson = JsonConvert.SerializeObject(obj);
                throw new AssertException(string.Format(
                    "assert is true failed: [{0}]\r\n  {1}:{2} {3}",
                    objJson,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if condition is false<br/>
        /// 测试条件是否为假<br/>
        /// </summary>
        public static void IsFalse(
            bool condition,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (condition)
            {
                throw new AssertException(string.Format(
                    "assert is false failed\r\n  {0}:{1} {2}",
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if condition is false<br/>
        /// If failed include `obj` as contents of exception message<br/>
        /// 测试条件是否为假<br/>
        /// 如果失败会包含obj的内容到例外消息<br/>
        /// </summary>
        public static void IsFalseWith(
            bool condition,
            object obj,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (condition)
            {
                var objJson = JsonConvert.SerializeObject(obj);
                throw new AssertException(string.Format(
                    "assert is false failed [{0}]\r\n  {1}:{2} {3}",
                    objJson,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if a and b are equals<br/>
        /// 测试a和b是否相等<br/>
        /// </summary>
        public static void Equals<T>(
            T a,
            T b,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!object.Equals(a, b))
            {
                throw new AssertException(string.Format(
                    "assert equals failed: [{0}] != [{1}]\r\n  {2}:{3} {4}",
                    a,
                    b,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if a and b are equals<br/>
        /// Use to avoid calling object.Equals<br/>
        /// 测试a和b是否相等<br/>
        /// 用于防止调用object.Equals<br/>
        /// </summary>
        public static void Equals(
            object a,
            object b,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            Equals<object>(a, b, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Test if a and b are not equals<br/>
        /// 测试a和b是否相等<br/>
        /// </summary>
        public static void NotEquals<T>(
            T a,
            T b,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (object.Equals(a, b))
            {
                throw new AssertException(string.Format(
                    "assert not equals failed: [{0}] == [{1}]\r\n  {2}:{3} {4}",
                    a,
                    b,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if text contains pattern<br/>
        /// 测试text是否包含pattern<br/>
        /// </summary>
        public static void Contains(
            string text,
            string pattern,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!text.Contains(pattern))
            {
                throw new AssertException(string.Format(
                    "assert contains failed: [{0}] not contains [{1}]\r\n  {2}:{3} {4}",
                    text,
                    pattern,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if text not contains pattern<br/>
        /// 测试text是否不包含pattern<br/>
        /// </summary>
        public static void NotContains(
            string text,
            string pattern,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (text.Contains(pattern))
            {
                throw new AssertException(string.Format(
                    "assert not contains failed: [{0}] contains [{1}]\r\n  {2}:{3} {4}",
                    text,
                    pattern,
                    filePath,
                    lineNumber,
                    memberName));
            }
        }

        /// <summary>
        /// Test if action throws specified type's exception<br/>
        /// 测试函数是否抛出了指定类型的异常<br/>
        /// </summary>
        public static void Throws<TException>(
            Action action,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
            where TException : Exception
        {
            ThrowsWith<TException>(action, null, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Test if action throws specified type's exception and contains specified message<br/>
        /// 测试函数是否抛出了指定类型的异常并包含指定消息<br/>
        /// </summary>
        public static void ThrowsWith<TException>(
            Action action,
            string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                if (!string.IsNullOrEmpty(message) && !ex.Message.Contains(message))
                {
                    throw new AssertException(string.Format(
                        "assert throws failed because message not matched: ex is {0}\r\n  {1}:{2} {3}",
                        ex,
                        filePath,
                        lineNumber,
                        memberName));
                }
                return;
            }
            catch (Exception ex)
            {
                throw new AssertException(string.Format(
                    "assert throws failed because type not matched: ex is {0}\r\n  {1}:{2} {3}",
                    ex,
                    filePath,
                    lineNumber,
                    memberName));
            }
            throw new AssertException(string.Format(
                "assert throws failed because no exception throwed\r\n  {0}:{1} {2}",
                filePath,
                lineNumber,
                memberName));
        }

        /// <summary>
        /// Make this test passed<br/>
        /// 让当前测试通过<br/>
        /// </summary>
        public static void Passed()
        {
            throw new AssertPassedException();
        }

        /// <summary>
        /// Make this test skipped<br/>
        /// 跳过当前测试<br/>
        /// </summary>
        public static void Skipped(string reason)
        {
            throw new AssertSkipedException(reason);
        }
    }
}
