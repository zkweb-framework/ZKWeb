using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http request<br/>
	/// Http请求的接口<br/>
	/// </summary>
	public interface IHttpRequest {
		/// <summary>
		/// Request content<br/>
		/// 请求内容<br/>
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content length<br/>
		/// 内容长度<br/>
		/// </summary>
		long? ContentLength { get; }
		/// <summary>
		/// Content type<br/>
		/// 内容类型<br/>
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// Request from hostname and port<br/>
		/// 请求的域名和端口<br/>
		/// </summary>
		/// <example>localhost:8765</example>
		string Host { get; }
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Is https<br/>
		/// 是否Https<br/>
		/// </summary>
		bool IsHttps { get; }
		/// <summary>
		/// Request method<br/>
		/// 请求的方法<br/>
		/// </summary>
		/// <example>GET</example>
		string Method { get; }
		/// <summary>
		/// Request protocol (eg: HTTP/1.1)<br/>
		/// 请求的协议 (例如: HTTP/1.1)<br/>
		/// </summary>
		/// <example>HTTP/1.1</example>
		string Protocol { get; }
		/// <summary>
		/// Request path<br/>
		/// 请求的路径<br/>
		/// </summary>
		/// <example>/test</example>
		string Path { get; }
		/// <summary>
		/// Request query string, startswith `?` if not empty<br/>
		/// Url中的参数字符串, 如果不为空则以?开始<br/>
		/// </summary>
		/// <example>?key=value</example>
		string QueryString { get; }
		/// <summary>
		/// Request scheme (eg: http)<br/>
		/// 请求的协议 (例如: http)<br/>
		/// </summary>
		/// <example>http</example>
		string Scheme { get; }
		/// <summary>
		/// Remote ip address<br/>
		/// 远程IP地址<br/>
		/// </summary>
		/// <example>127.0.0.1</example>
		IPAddress RemoteIpAddress { get; }
		/// <summary>
		/// Remote port<br/>
		/// 远程端口<br/>
		/// </summary>
		/// <example>65535</example>
		int RemotePort { get; }
		/// <summary>
		/// Custom parameters, can use to override other parameters(query, form, ...)<br/>
		/// 自定义参数, 可用于重写其他参数(请求, 表单, ...)<br/>
		/// </summary>
		IDictionary<string, object> CustomParameters { get; }

		/// <summary>
		/// Get cookie value<br/>
		/// 获取Cookie值<br/>
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		string GetCookie(string key);
		/// <summary>
		/// Get all cookie keys and values<br/>
		/// 获取所有Cookie键和值<br/>
		/// </summary>
		IEnumerable<Pair<string, string>> GetCookies();
		/// <summary>
		/// Get query value<br/>
		/// Return null if not exist<br/>
		/// 获取查询参数值<br/>
		/// 不存在时返回null<br/>
		/// </summary>
		/// <param name="key">Query key</param>
		/// <returns></returns>
		IList<string> GetQueryValue(string key);
		/// <summary>
		/// Get all query keys and values<br/>
		/// 获取所有查询参数键和值<br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetQueryValues();
		/// <summary>
		/// Get form value<br/>
		/// Return null if not exist<br/>
		/// 获取表单值<br/>
		/// 不存在时返回null<br/>
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IList<string> GetFormValue(string key);
		/// <summary>
		/// Get all form keys and values<br/>
		/// 获取所有表单键和值<br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetFormValues();
		/// <summary>
		/// Get http header value<br/>
		/// Return null if not exist<br/>
		/// 获取Http头的值<br/>
		/// 不存在时返回null<br/>
		/// </summary>
		/// <param name="key">Header key</param>
		/// <returns></returns>
		string GetHeader(string key);
		/// <summary>
		/// Get all http header keys and values<br/>
		/// 获取所有Http头的键和值<br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, string>> GetHeaders();
		/// <summary>
		/// Get posted file<br/>
		/// Return null if not exist<br/>
		/// 获取提交的文件<br/>
		/// 不存在时返回null<br/>
		/// </summary>
		/// <param name="key">Form key</param>
		/// <returns></returns>
		IHttpPostedFile GetPostedFile(string key);
		/// <summary>
		/// Get all posted files<br/>
		/// 获取所有提交的文件<br/>
		/// </summary>
		IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles();
	}
}
