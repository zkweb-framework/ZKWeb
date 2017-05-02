using System;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// Interface for application class
	/// </summary>
	public interface IApplication {
		/// <summary>
		/// ZKWeb Version String
		/// </summary>
		string FullVersion { get; }
		/// <summary>
		/// ZKWeb Version Object
		/// </summary>
		Version Version { get; }
		/// <summary>
		/// The IoC Container Instance
		/// </summary>
		IContainer Ioc { get; }
		/// <summary>
		/// In progress requests
		/// </summary>
		int InProgressRequests { get; }

		/// <summary>
		/// Intialize main application
		/// </summary>
		/// <param name="websiteRootDirectory"></param>
		void Initialize(string websiteRootDirectory);

		/// <summary>
		/// Handle http request
		/// `Response.End` will be called if processing completed without errors
		/// </summary>
		/// <param name="context">Http context</param>
		void OnRequest(IHttpContext context);

		/// <summary>
		/// Handle http error
		/// `Response.End` will be called if processing completed without errors
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="ex">Exception object</param>
		void OnError(IHttpContext context, Exception ex);

		/// <summary>
		/// Override IoC container, only available for the thread calling this method
		/// Overrided container will inherit original container,
		/// Alter overrided container will not affect original container.
		/// </summary>
		/// <returns></returns>
		IDisposable OverrideIoc();
	}
}
