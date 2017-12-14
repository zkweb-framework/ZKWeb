using System;

namespace ZKWeb.Hosting.Owin {
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	/// <summary>
	/// Http response end exception for Owin<br/>
	/// Owin中表示Http回应结束的例外<br/>
	/// </summary>
	public class OwinHttpResponseEndException : Exception { }
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
}