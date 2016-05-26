using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using DryIoc;
using ZKWeb.Utils.Extensions;
using ZKWeb.Database;
using ZKWeb.Web.Interfaces;
using ZKWeb.Plugin;
using ZKWeb.Logging;
using ZKWeb.Plugin.Interfaces;
using ZKWeb.Templating;
using ZKWeb.Serialize;
using ZKWeb.Web;
using ZKWeb.Server;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Localize.JsonConverters;
using ZKWeb.Localize;
using ZKWeb.UnitTest;
using System.Threading;
using ZKWeb.Utils.Collections;

namespace ZKWeb {
	/// <summary>
	/// 网站程序
	/// 用于初始化网站和提供当前使用的容器
	/// 
	/// 目前组件的依赖都不通过构造函数注入，尽管DryIoC本身支持多种注入（Func和Lazy等）。
	/// 通过构造函数注入的优缺点有
	/// 优点
	///		在注册时可以确认所有依赖的生成函数，创建对象的性能会比手动获取快30%以上（实测）
	///		单元测试时不需要依赖全局对象，可以使用xunit和nunit等工具单独测试
	///		单元测试时可以手动指定依赖的所有组件
	///	缺点
	///		需要编写大量多余的代码，继承时需要重新把所有注入项复制一遍
	///		单例的组件会把依赖保存到成员中，单例创建后无法影响这些依赖项（使用Func可解决，但需要预见这种情况）
	///		对泛型支持不好，例如Context.GetTable[T]这种函数无法预先注入依赖项，只能先注入整个容器
	/// 目前整个项目和插件项目都使用了手动获取而不是自动注入，
	/// 原因是可以减少代码量，并且单元测试时可以通过OverrideIoc统一处理。
	/// 单元测试时使用OverrideIoc可以单独替换指定的依赖项，不需要手动注入所有的依赖项。
	/// </summary>
	public class Application : HttpApplication {
		/// <summary>
		/// 框架的完整版本
		/// </summary>
		public static string FullVersion { get { return "0.9.3 testing"; } }
		/// <summary>
		/// 框架的数值版本
		/// </summary>
		public static Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// 当前使用的Ioc容器
		/// </summary>
		public static IContainer Ioc { get { return _overrideIoc.Value ?? _globalIoc; } }
		private static IContainer _globalIoc = new Container();
		private static ThreadLocal<IContainer> _overrideIoc = new ThreadLocal<IContainer>();

		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		public void Application_Start() {
			// 注册管理器类型
			Ioc.RegisterMany<DatabaseManager>(Reuse.Singleton);
			Ioc.RegisterMany<TJsonConverter>(Reuse.Singleton);
			Ioc.RegisterMany<TranslateManager>(Reuse.Singleton);
			Ioc.RegisterMany<LogManager>(Reuse.Singleton);
			Ioc.RegisterMany<PluginManager>(Reuse.Singleton);
			Ioc.RegisterMany<PluginReloader>(Reuse.Singleton);
			Ioc.RegisterMany<InitializeJsonNet>(Reuse.Singleton);
			Ioc.RegisterMany<ConfigManager>(Reuse.Singleton);
			Ioc.RegisterMany<PathManager>(Reuse.Singleton);
			Ioc.RegisterMany<TemplateAreaManager>(Reuse.Singleton);
			Ioc.RegisterMany<TemplateFileSystem>(Reuse.Singleton);
			Ioc.RegisterMany<TemplateManager>(Reuse.Singleton);
			Ioc.RegisterMany<UnitTestManager>(Reuse.Singleton);
			Ioc.RegisterMany<ControllerManager>(Reuse.Singleton);
			// 初始化管理器
			Ioc.Resolve<PluginManager>();
			Ioc.Resolve<TemplateManager>();
			Ioc.Resolve<ControllerManager>();
			Ioc.Resolve<InitializeJsonNet>();
			Ioc.Resolve<DatabaseManager>();
			// 初始化所有插件并调用网站启动时的处理
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
			// 自动重新载入插件和网站配置
			Ioc.Resolve<PluginReloader>();
		}

		/// <summary>
		/// 收到Http请求时的处理
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			// 调用预处理器，先注册的先调用
			foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>()) {
				handler.OnRequest();
			}
			// 调用处理器，后注册的先调用
			foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
				handler.OnRequest();
			}
			// 没有处理时返回404
			throw new HttpException(404, "404 Not Found");
		}

		/// <summary>
		/// 捕获到例外时的处理
		/// 注意这个函数执行时使用的Application可能和初始化的不一样
		/// 获取Ioc成员时应该通过Current.Ioc
		/// </summary>
		protected void Application_Error(object sender, EventArgs e) {
			// 获取最后抛出的例外并记录到日志
			var ex = Server.GetLastError();
			if (ex is HttpUnhandledException && ex.InnerException != null) {
				ex = ex.InnerException;
			}
			// 记录到日志
			// 不记录400~499之间的错误（客户端错误）
			var logManager = Ioc.Resolve<LogManager>();
			var httpCode = (ex as HttpException)?.GetHttpCode();
			if (!(httpCode >= 400 && httpCode < 500)) {
				logManager.LogError(ex.ToString());
			}
			// 判断是否启动程序时抛出的错误，如果是则卸载程序域等待重试
			try {
				Response.StatusCode = Response.StatusCode;
			} catch {
				HttpRuntime.UnloadAppDomain();
				return;
			}
			// 调用回调处理错误
			// 如回调中重定向或结束请求的处理，会抛出ThreadAbortException
			var handlers = Ioc.ResolveMany<IHttpErrorHandler>();
			handlers.Reverse().ForEach(h => h.OnError(ex));
			// 如果是ajax请求则只返回例外消息
			var requestBase = new HttpRequestWrapper(Request);
			if (requestBase.IsAjaxRequest()) {
				Response.StatusCode = httpCode ?? 500;
				Response.Write(ex.Message);
				Response.End();
			}
		}

		/// <summary>
		/// 重载当前使用的Ioc容器，在当前线程中有效
		/// 重载后的容器会继承原有的容器，但不会对原有的容器做出修改
		/// </summary>
		/// <returns></returns>
		public static IDisposable OverrideIoc() {
			var original = _overrideIoc.Value;
			_overrideIoc.Value = Ioc.CreateFacade();
			return new SimpleDisposable(() => _overrideIoc.Value = original);
		}
	}
}
