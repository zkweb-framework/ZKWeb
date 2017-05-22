using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http request<br/>
	/// <br/>
	/// </summary>
	public interface IHttpRequest {
		/// <summary>
		/// Request content<br/>
		/// <br/>
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content length<br/>
		/// <br/>
		/// </summary>
		long? ContentLength { get; }
		/// <summary>
		/// Content type<br/>
		/// <br/>
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// Request from hostname and port<br/>
		/// <br/>
		/// </summary>
		/// <example>localhost:8765</example>
		string Host { get; }
		/// <summary>
		/// Parent http context<br/>
		/// <br/>
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Is https<br/>
		/// <br/>
		/// </summary>
		bool IsHttps { get; }
		/// <summary>
		/// Request method<br/>
		/// <br/>
		/// </summary>
		/// <example>GET</example>
		string Method { get; }
		/// <summary>
		/// Request protocol (eg: HTTP/1.1)<br/>
		/// <br/>
		/// </summary>
		/// <example>HTTP/1.1</example>
		string Protocol { get; }
		/// <summary>
		/// Request path<br/>
		/// <br/>
		/// </summary>
		/// <example>/test</example>
		string Path { get; }
		/// <summary>
		/// Request query string, startswith `?` if not empty<br/>
		/// <br/>
		/// </summary>
		/// <example>?key=value</example>
		string QueryString { get; }
		/// <summary>
		/// Request scheme (eg: http)<br/>
		/// <br/>
		/// </summary>
		/// <example>http</example>
		string Scheme { get; }
		/// <summary>
		/// Remote ip address<br/>
		/// <br/>
		/// </summary>
		/// <example>127.0.0.1</example>
		IPAddress RemoteIpAddress { get; }
		/// <summary>
		/// Remote port<br/>
		/// <br/>
		/// </summary>
		/// <example>65535</example>
		int RemotePort { get; }

		/// <summary>
		/// Get cookie value<br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		string GetCookie(string key);
		/// <summary>
		/// Get all cookie keys and values<br/>
		/// <br/>
		/// </summary>
		IEnumerable<Pair<string, string>> GetCookies();
		/// <summary>
		/// Get query value<br/>
		/// Return null if not exist<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Query key</param>
		/// <returns></returns>
		IList<string> GetQueryValue(string key);
		/// <summary>
		/// Get all query keys and values<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetQueryValues();
		/// <summary>
		/// Get form value<br/>
		/// Return null if not exist<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IList<string> GetFormValue(string key);
		/// <summary>
		/// Get all form keys and values<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetFormValues();
		/// <summary>
		/// Get http header value<br/>
		/// Return null if not exist<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Header key</param>
		/// <returns></returns>
		string GetHeader(string key);
		/// <summary>
		/// Get all http header keys and values<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, string>> GetHeaders();
		/// <summary>
		/// Get posted file<br/>
		/// Return null if not exist<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IHttpPostedFile GetPostedFile(string key);
		/// <summary>
		/// Get all posted files<br/>
		/// <br/>
		/// </summary>
		IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles();
	}
}
