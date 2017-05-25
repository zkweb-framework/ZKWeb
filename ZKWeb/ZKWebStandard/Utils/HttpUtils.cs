using System.Collections.Generic;
using System.Net;
using System.Text;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Http utility functions<br/>
	/// Http的工具函数<br/>
	/// </summary>
	public static class HttpUtils {
		/// <summary>
		/// Url encode<br/>
		/// 编码到Url<br/>
		/// </summary>
		/// <param name="value">Original value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpUtils.UrlEncode("'&amp;^%=abc") == "%27%26%5E%25%3Dabc"
		/// </code>
		/// </example>
		public static string UrlEncode(object value) {
			return WebUtility.UrlEncode(value?.ToString() ?? "");
		}

		/// <summary>
		/// Url decode<br/>
		/// 从Url解码<br/>
		/// </summary>
		/// <param name="value">Original value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpUtils.UrlDecode("%27%26%5E%25%3Dabc") == "'&amp;^%=abc"
		/// </code>
		/// </example>
		public static string UrlDecode(string value) {
			return WebUtility.UrlDecode(value ?? "");
		}

		/// <summary>
		/// Html encode<br/>
		/// 编码到Html<br/>
		/// </summary>
		/// <param name="value">Original value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpUtils.HtmlEncode("asd'\"&lt;&gt;") == "asd&amp;#39;&amp;quot;&amp;lt;&amp;gt;"
		/// </code>
		/// </example>
		public static string HtmlEncode(object value) {
			return WebUtility.HtmlEncode(value?.ToString() ?? "");
		}

		/// <summary>
		/// Html decode<br/>
		/// 从Html解码<br/>
		/// </summary>
		/// <param name="value">Original value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpUtils.HtmlDecode("asd&amp;#39;&amp;quot;&amp;lt;&amp;gt;"), "asd'\"&lt;&gt;"
		/// </code>
		/// </example>
		public static string HtmlDecode(string value) {
			return WebUtility.HtmlDecode(value ?? "");
		}

		/// <summary>
		/// Split path and query string<br/>
		/// 分割路径和查询字符串<br/>
		/// </summary>
		/// <param name="pathAndQuery">Path an query string</param>
		/// <param name="path">Path</param>
		/// <param name="queryString">Query string</param>
		/// <example>
		/// <code language="cs">
		/// string path;
		/// string query;
		/// HttpUtils.SplitPathAndQuery("test", out path, out query);
		/// // path == "test"
		/// // query == ""
		/// 
		/// HttpUtils.SplitPathAndQuery("test?a=1&amp;b=2", out path, out query);
		/// // path == "test"
		/// // query == "?a=1&amp;b=2"
		/// </code>
		/// </example>
		public static void SplitPathAndQuery(
			string pathAndQuery, out string path, out string queryString) {
			var queryIndex = pathAndQuery.IndexOf('?');
			path = (queryIndex >= 0) ? pathAndQuery.Substring(0, queryIndex) : pathAndQuery;
			queryString = (queryIndex >= 0) ? pathAndQuery.Substring(queryIndex) : "";
		}

		/// <summary>
		/// Parse query string<br/>
		/// 解析查询字符串<br/>
		/// </summary>
		/// <param name="queryString">Query string</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var result = HttpUtils.ParseQueryString("a=1&amp;b=2&amp;key=%27%26%5E%25%3Dabc");
		/// result.Count == 3
		/// result["a"].Count == 1
		/// result["a"][0] == "1"
		/// result["b"].Count == 1
		/// result["b"][0] == "2"
		/// result["key"].Count == 1
		/// result["key"][0] == "'&amp;^%=abc"
		/// </code>
		/// </example>
		public static IDictionary<string, IList<string>> ParseQueryString(string queryString) {
			var result = new Dictionary<string, IList<string>>();
			if (string.IsNullOrEmpty(queryString)) {
				return result;
			}
			// Trim beginning `?`
			var startIndex = (queryString[0] == '?') ? 1 : 0;
			// Find all keys and values
			while (startIndex < queryString.Length) {
				var equalIndex = queryString.IndexOf('=', startIndex);
				if (equalIndex < 0) {
					return result;
				}
				var andIndex = queryString.IndexOf('&', equalIndex);
				if (andIndex < 0) {
					andIndex = queryString.Length;
				}
				var key = UrlDecode(queryString.Substring(startIndex, equalIndex - startIndex));
				var value = UrlDecode(queryString.Substring(equalIndex + 1, andIndex - equalIndex - 1));
				startIndex = andIndex + 1;
				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
					result.GetOrCreate(key, () => new List<string>()).Add(value);
				}
			}
			return result;
		}

		/// <summary>
		/// Build query string<br/>
		/// 构建查询字符串<br/>
		/// </summary>
		/// <param name="queryParams">Query arguments</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var query = new Dictionary&lt;string, IList&lt;string&gt;&gt;();
		/// query["name"] = new[] { "john", "harold" };
		/// query["age"] = new[] { "50&amp;51" };
		/// // result == "name=john&amp;name=harold&amp;age=50%2651"
		/// </code>
		/// </example>
		public static string BuildQueryString(IDictionary<string, IList<string>> queryParams) {
			var queryString = new StringBuilder();
			var isFirst = true;
			foreach (var pair in queryParams) {
				foreach (var value in pair.Value) {
					if (!isFirst) {
						queryString.Append('&');
					}
					isFirst = false;
					queryString.Append(UrlEncode(pair.Key));
					queryString.Append('=');
					queryString.Append(UrlEncode(value));
				}
			}
			return queryString.ToString();
		}
	}
}
