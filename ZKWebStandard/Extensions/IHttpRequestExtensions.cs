using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http请求类的扩展函数
	/// </summary>
	public static class IHttpRequestExtensions {
		/// <summary>
		/// 判断http请求是否由ajax发起
		/// </summary>
		/// <param name="request">http请求</param>
		/// <returns></returns>
		public static bool IsAjaxRequest(this IHttpRequest request) {
			return request.GetHeader("X-Requested-With") == "XMLHttpRequest";
		}

		/// <summary>
		/// 获取请求的UserAgent
		/// </summary>
		/// <param name="request">Http请求</param>
		/// <returns></returns>
		public static string GetUserAgent(this IHttpRequest request) {
			return request.GetHeader("User-Agent");
		}

		/// <summary>
		/// 获取请求要求的语言列表
		/// </summary>
		/// <param name="request">Http请求</param>
		/// <returns></returns>
		public static IList<string> GetAcceptLanguages(this IHttpRequest request) {
			// TODO: 测试
			// Accept-Language:"en-US,en;q=0.7,zh-CN;q=0.3"
			var acceptLanguages = request.GetHeader("Accept-Language") ?? "";
			var result = acceptLanguages.Split(',').Select(s => s.Split(';')[0]).ToList();
			return result;
		}

		/// <summary>
		/// 获取客户端中缓存的最后修改的时间
		/// </summary>
		/// <param name="request">Http请求</param>
		/// <returns></returns>
		public static DateTime GetIfModifiedSince(this IHttpRequest request) {
			var value = request.GetHeader("If-Modified-Since");
			if (string.IsNullOrEmpty(value)) {
				return DateTime.MinValue;
			}
			DateTime result;
			if (!DateTime.TryParseExact(value, "R",
				DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out result)) {
				return DateTime.MinValue;
			}
			return result.ToUniversalTime();
		}

		/// <summary>
		/// 获取http请求中的指定参数值
		/// 优先级: 表单内容 > 请求字符串
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="request">Http请求</param>
		/// <param name="key">参数名称</param>
		/// <param name="defaultValue">获取失败时的默认值</param>
		/// <returns></returns>
		public static T Get<T>(this IHttpRequest request, string key, T defaultValue = default(T)) {
			var values = request.GetFormValue(key);
			if (values == null || values.Count <= 0) {
				values = request.GetQueryValue(key);
			}
			if (values == null || values.Count <= 0) {
				return defaultValue;
			}
			return values[0].ConvertOrDefault<T>(defaultValue);
		}

		/// <summary>
		/// 获取http请求中的所有参数
		/// 优先级: 表单内容 > 请求字符串
		/// </summary>
		/// <param name="request">Http请求</param>
		/// <returns></returns>
		public static IEnumerable<Pair<string, string>> GetAll(this IHttpRequest request) {
			foreach (var pair in request.GetFormValues()) {
				yield return Pair.Create(pair.First, pair.Second[0]);
			}
			foreach (var pair in request.GetQueryValues()) {
				yield return Pair.Create(pair.First, pair.Second[0]);
			}
		}
	}
}
