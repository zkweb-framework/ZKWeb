using System;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// Interface of application class<br/>
	/// 应用类的接口<br/>
	/// </summary>
	public interface IApplication {
		/// <summary>
		/// ZKWeb Version String<br/>
		/// ZKWeb的版本字符串<br/>
		/// </summary>
		string FullVersion { get; }
		/// <summary>
		/// ZKWeb Version Object<br/>
		/// ZKWeb的版本对象<br/>
		/// </summary>
		Version Version { get; }
		/// <summary>
		/// The IoC Container Instance<br/>
		/// IoC容器的实例<br/>
		/// </summary>
		IContainer Ioc { get; }
		/// <summary>
		/// In progress requests<br/>
		/// 处理中的请求数量<br/>
		/// </summary>
		int InProgressRequests { get; }

		/// <summary>
		/// Intialize main application<br/>
		/// 初始化主应用<br/>
		/// </summary>
		/// <param name="websiteRootDirectory"></param>
		void Initialize(string websiteRootDirectory);

		/// <summary>
		/// Handle http request<br/>
		/// Method "Response.End" will be called if processing completed without errors
		/// 处理Http请求<br/>
		/// 如果处理成功完成将会调用"Response.End"函数<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		void OnRequest(IHttpContext context);

		/// <summary>
		/// Handle http error<br/>
		/// Method "Response.End" will be called if processing completed without errors<br/>
		/// 处理Http错误<br/>
		/// 如果处理成功完成将会调用"Response.End"函数<br/> 
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="ex">Exception object</param>
		void OnError(IHttpContext context, Exception ex);

		/// <summary>
		/// Override IoC container, only affect the thread calling this method<br/>
		/// Overrided container will inherit original container,<br/>
		/// Alter overrided container will not affect original container.<br/>
		/// 重载当前线程中的IoC容器, 只影响调用此函数的线程<br/>
		/// 重载后的容器会继承原始容器的内容, 并且修改重载后的容器不会影响原始容器<br/>
		/// </summary>
		/// <returns></returns>
		IDisposable OverrideIoc();
	}
}
