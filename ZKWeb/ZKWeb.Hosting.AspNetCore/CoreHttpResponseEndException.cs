using System;

namespace ZKWeb.Hosting.AspNetCore {
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	/// <summary>
	/// Reponse end exception for Asp.Net Core<br/>
	/// Asp.Net Core使用的代表请求结束的例外<br/>
	/// </summary>
	public class CoreHttpResponseEndException : Exception { }
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
}
