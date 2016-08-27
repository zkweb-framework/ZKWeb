using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http request extension methods
	/// </summary>
	public static class IHttpRequestExtensions {
		/// <summary>
		/// Determine it's a ajax http request
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static bool IsAjaxRequest(this IHttpRequest request) {
			return request.GetHeader("X-Requested-With") == "XMLHttpRequest";
		}

		/// <summary>
		/// Get user agent from http request
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static string GetUserAgent(this IHttpRequest request) {
			return request.GetHeader("User-Agent");
		}

		/// <summary>
		/// Get accept languages from http request
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static IList<string> GetAcceptLanguages(this IHttpRequest request) {
			var acceptLanguages = request.GetHeader("Accept-Language") ?? "";
			var result = acceptLanguages.Split(',').Select(s => s.Split(';')[0]).ToList();
			return result;
		}

		/// <summary>
		/// Get "If-Modified-Since" header's value from http request
		/// Return DateTime.MinValue if not found
		/// </summary>
		/// <param name="request">Http request</param>
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
		/// Get Referer from http request
		/// Return null if not exist
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static Uri GetReferer(this IHttpRequest request) {
			var referer = request.GetHeader("Referer");
			if (referer == null) {
				return null;
			}
			Uri refererUri;
			if (!Uri.TryCreate(referer, UriKind.Absolute, out refererUri)) {
				return null;
			}
			return refererUri;
		}

		/// <summary>
		/// Get argument from http request
		/// Priority: Form > QueryString
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		/// <param name="request">Http request</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
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
		/// Get all arguments from http request
		/// Priority: Form > QueryString
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static IEnumerable<Pair<string, IList<string>>> GetAll(this IHttpRequest request) {
			foreach (var pair in request.GetFormValues()) {
				yield return Pair.Create(pair.First, pair.Second);
			}
			foreach (var pair in request.GetQueryValues()) {
				yield return Pair.Create(pair.First, pair.Second);
			}
		}

		/// <summary>
		/// Get all arguments from http request in dictionary
		/// Priority: Form > QueryString
		/// </summary>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static IDictionary<string, IList<string>> GetAllDictionary(this IHttpRequest request) {
			var result = new Dictionary<string, IList<string>>();
			foreach (var pair in request.GetAll()) {
				if (!result.ContainsKey(pair.First)) {
					result[pair.First] = pair.Second;
				}
			}
			return result;
		}

		/// <summary>
		/// Get all parameters into a given type
		/// </summary>
		/// <typeparam name="T">The type contains parameters</typeparam>
		/// <param name="request">Http request</param>
		/// <returns></returns>
		public static T GetAllAs<T>(this IHttpRequest request) {
			if (request.ContentType?.StartsWith("application/json") ?? false) {
				// Deserialize with json
				// It should only read once
				var json = (string)request.HttpContext.Items.GetOrCreate(
					"__json_body", () => new StreamReader(request.Body).ReadToEnd());
				return JsonConvert.DeserializeObject<T>(json);
			} else if (typeof(T) == typeof(IDictionary<string, object>) ||
				typeof(T) == typeof(Dictionary<string, object>)) {
				// Return all parameters
				return (T)(object)request.GetAllDictionary().ToDictionary(
					p => p.Key, p => (object)p.Value.FirstOrDefault());
			} else if (typeof(T) == typeof(IDictionary<string, string>) ||
				typeof(T) == typeof(Dictionary<string, string>)) {
				// Return all parameters
				return (T)(object)request.GetAllDictionary().ToDictionary(
					p => p.Key, p => (string)p.Value.FirstOrDefault());
			} else {
				// Get each property by it's name
				var value = (T)Activator.CreateInstance(typeof(T));
				foreach (var property in typeof(T).FastGetProperties()) {
					if (!property.CanRead || !property.CanWrite) {
						continue; // Property is read or write only
					}
					object propertyValue;
					if (property.PropertyType == typeof(IHttpPostedFile)) {
						propertyValue = request.GetPostedFile(property.Name);
					} else {
						propertyValue = request.Get<string>(property.Name)
							.ConvertOrDefault(property.PropertyType, null);
					}
					if (propertyValue != null) {
						property.FastSetValue(value, propertyValue);
					}
				}
				return value;
			}
		}
	}
}
