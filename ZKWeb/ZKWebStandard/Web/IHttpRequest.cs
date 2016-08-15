using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http request
	/// </summary>
	public interface IHttpRequest {
		/// <summary>
		/// Request content
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content length
		/// </summary>
		long? ContentLength { get; }
		/// <summary>
		/// Content type
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// Request from hostname and port
		/// </summary>
		/// <example>localhost:8765</example>
		string Host { get; }
		/// <summary>
		/// Parent http context
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Is https
		/// </summary>
		bool IsHttps { get; }
		/// <summary>
		/// Request method
		/// </summary>
		/// <example>GET</example>
		string Method { get; }
		/// <summary>
		/// Request protocol (eg: HTTP/1.1)
		/// </summary>
		/// <example>HTTP/1.1</example>
		string Protocol { get; }
		/// <summary>
		/// Request path
		/// </summary>
		/// <example>/test</example>
		string Path { get; }
		/// <summary>
		/// Request query string, startswith `?` if not empty
		/// </summary>
		/// <example>?key=value</example>
		string QueryString { get; }
		/// <summary>
		/// Request scheme (eg: http)
		/// </summary>
		/// <example>http</example>
		string Scheme { get; }
		/// <summary>
		/// Remote ip address
		/// </summary>
		/// <example>127.0.0.1</example>
		IPAddress RemoteIpAddress { get; }
		/// <summary>
		/// Remote port
		/// </summary>
		/// <example>65535</example>
		int RemotePort { get; }

		/// <summary>
		/// Get cookie value
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		string GetCookie(string key);
		/// <summary>
		/// Get all cookie keys and values
		/// </summary>
		IEnumerable<Pair<string, string>> GetCookies();
		/// <summary>
		/// Get query value
		/// Return null if not exist
		/// </summary>
		/// <param name="key">Query key</param>
		/// <returns></returns>
		IList<string> GetQueryValue(string key);
		/// <summary>
		/// Get all query keys and values
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetQueryValues();
		/// <summary>
		/// Get form value
		/// Return null if not exist
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IList<string> GetFormValue(string key);
		/// <summary>
		/// Get all form keys and values
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetFormValues();
		/// <summary>
		/// Get http header value
		/// Return null if not exist
		/// </summary>
		/// <param name="key">Header key</param>
		/// <returns></returns>
		string GetHeader(string key);
		/// <summary>
		/// Get all http header keys and values
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, string>> GetHeaders();
		/// <summary>
		/// Get posted file
		/// Return null if not exist
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IHttpPostedFile GetPostedFile(string key);
		/// <summary>
		/// Get all posted files
		/// </summary>
		IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles();
	}
}
