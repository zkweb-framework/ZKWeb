using System;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb {
	/// <summary>
	/// Main Application
	/// </summary>
	public static class Application {
		/// <summary>
		/// Application Instance
		/// </summary>
		public static IApplication Instance {
			get {
				if (instance == null) {
					throw new NullReferenceException("Please set Application.Instance first");
				}
				return instance;
			}
			set { instance = value; }
		}
		private static IApplication instance;
		/// <summary>
		/// ZKWeb Version String
		/// </summary>
		public static string FullVersion { get { return Instance.FullVersion; } }
		/// <summary>
		/// ZKWeb Version Object
		/// </summary>
		public static Version Version { get { return Instance.Version; } }
		/// <summary>
		/// The IoC Container Instance
		/// </summary>
		public static IContainer Ioc { get { return Instance.Ioc; } }
		/// <summary>
		/// In progress requests
		/// </summary>
		public static int InProgressRequests { get { return Instance.InProgressRequests; } }

		/// <summary>
		/// Intialize application with DefaultApplication
		/// </summary>
		public static void Initialize(string websiteRootDirectory) {
			Initialize<DefaultApplication>(websiteRootDirectory);
		}

		/// <summary>
		/// Intialize application
		/// </summary>
		public static void Initialize<TApplication>(string websiteRootDirectory)
			where TApplication : IApplication, new() {
			var application = new TApplication();
			application.Initialize(websiteRootDirectory);
			Instance = application;
		}

		/// <summary>
		/// Handle http request
		/// </summary>
		public static void OnRequest(IHttpContext context) {
			Instance.OnRequest(context);
		}

		/// <summary>
		/// Handle http error
		/// </summary>
		public static void OnError(IHttpContext context, Exception ex) {
			Instance.OnError(context, ex);
		}

		/// <summary>
		/// Override IoC container
		/// </summary>
		public static IDisposable OverrideIoc() {
			return Instance.OverrideIoc();
		}
	}
}
