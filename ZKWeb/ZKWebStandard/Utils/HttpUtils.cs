using System.Collections.Generic;
using System.Net;
using System.Text;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Http工具类
	/// </summary>
	public static class HttpUtils {
		/// <summary>
		/// Url编码
		/// </summary>
		/// <param name="value">字符串值</param>
		/// <returns></returns>
		public static string UrlEncode(object value) {
			return WebUtility.UrlEncode(value?.ToString() ?? "");
		}

		/// <summary>
		/// Url解码
		/// </summary>
		/// <param name="value">字符串值</param>
		/// <returns></returns>
		public static string UrlDecode(string value) {
			return WebUtility.UrlDecode(value ?? "");
		}

		/// <summary>
		/// Html编码
		/// </summary>
		/// <param name="value">字符串值</param>
		/// <returns></returns>
		public static string HtmlEncode(object value) {
			return WebUtility.HtmlEncode(value?.ToString() ?? "");
		}

		/// <summary>
		/// Html解码
		/// </summary>
		/// <param name="value">字符串值</param>
		/// <returns></returns>
		public static string HtmlDecode(string value) {
			return WebUtility.HtmlDecode(value ?? "");
		}

		/// <summary>
		/// 分割路径和请求字符串
		/// </summary>
		/// <param name="pathAndQuery">路径和请求字符串</param>
		/// <param name="path">路径</param>
		/// <param name="queryString">请求字符串</param>
		public static void SplitPathAndQuery(
			string pathAndQuery, out string path, out string queryString) {
			var queryIndex = pathAndQuery.IndexOf('?');
			path = (queryIndex >= 0) ? pathAndQuery.Substring(0, queryIndex) : pathAndQuery;
			queryString = (queryIndex >= 0) ? pathAndQuery.Substring(queryIndex) : "";
		}

		/// <summary>
		/// 解析请求字符串
		/// </summary>
		/// <param name="queryString">请求字符串</param>
		/// <returns></returns>
		public static IDictionary<string, IList<string>> ParseQueryString(string queryString) {
			var result = new Dictionary<string, IList<string>>();
			// 空白时不需要解析
			if (string.IsNullOrEmpty(queryString)) {
				return result;
			}
			// 以?开始时跳过这个字符
			var startIndex = (queryString[0] == '?') ? 1 : 0;
			// 获取所有键值
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
		/// 构建请求字符串
		/// </summary>
		/// <param name="queryParams">请求参数</param>
		/// <returns></returns>
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
