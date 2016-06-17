using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http请求的接口
	/// </summary>
	public interface IHttpRequest {
		/// <summary>
		/// 请求的数据流
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// 内容长度
		/// </summary>
		long? ContentLength { get; }
		/// <summary>
		/// 内容类型
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// 接收到请求的主机名和端口
		/// </summary>
		/// <example>localhost:8765</example>
		string Host { get; }
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// 是否Https
		/// </summary>
		bool IsHttps { get; }
		/// <summary>
		/// 请求类型
		/// </summary>
		/// <example>GET</example>
		string Method { get; }
		/// <summary>
		/// 请求协议，Tcp中的协议
		/// </summary>
		/// <example>HTTP/1.1</example>
		string Protocol { get; }
		/// <summary>
		/// 请求路径
		/// </summary>
		/// <example>/test</example>
		string Path { get; }
		/// <summary>
		/// 请求参数
		/// </summary>
		/// <example>?key=value</example>
		string QueryString { get; }
		/// <summary>
		/// 请求协议，Url中的协议
		/// </summary>
		/// <example>http</example>
		string Scheme { get; }
		/// <summary>
		/// 远程IP地址
		/// </summary>
		/// <example>127.0.0.1</example>
		IPAddress RemoteIpAddress { get; }
		/// <summary>
		/// 远程端口
		/// </summary>
		/// <example>65535</example>
		int RemotePort { get; }

		/// <summary>
		/// 获取Cookie值
		/// </summary>
		/// <param name="key">键名</param>
		/// <returns></returns>
		string GetCookie(string key);
		/// <summary>
		/// 获取所有Cookie键值
		/// </summary>
		IEnumerable<Pair<string, string>> GetCookies();
		/// <summary>
		/// 获取请求参数的值
		/// 获取不到时返回null
		/// </summary>
		/// <param name="key">键名</param>
		/// <returns></returns>
		IList<string> GetQueryValue(string key);
		/// <summary>
		/// 获取所有请求参数的键值
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetQueryValues();
		/// <summary>
		/// 获取表单值
		/// 获取不到时返回null
		/// </summary>
		/// <param name="key">键名</param>
		/// <returns></returns>
		IList<string> GetFormValue(string key);
		/// <summary>
		/// 获取所有表单的键值
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, IList<string>>> GetFormValues();
		/// <summary>
		/// 获取Http头的值
		/// 获取不到时返回null
		/// </summary>
		/// <param name="key">键名</param>
		/// <returns></returns>
		string GetHeader(string key);
		/// <summary>
		/// 获取所有Http头的键值
		/// </summary>
		/// <returns></returns>
		IEnumerable<Pair<string, string>> GetHeaders();
		/// <summary>
		/// 获取上传的文件
		/// 获取不到时返回null
		/// </summary>
		/// <param name="key">键名</param>
		/// <returns></returns>
		IHttpPostedFile GetPostedFile(string key);
		/// <summary>
		/// 获取所有上传的文件的键名和文件对象
		/// </summary>
		IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles();
	}
}
