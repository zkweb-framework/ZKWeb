using Microsoft.Owin;
using Owin;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Owin.Hosting;
using ZKWebStandard.Extensions;

namespace ZKWeb.Owin {
	/// <summary>
	/// Owin程序入口点的基类
	/// </summary>
	public class StartupBase {
		/// <summary>
		/// 获取网站根目录
		/// </summary>
		/// <returns></returns>
		public static string GetWebsiteRootDirectory() {
			if (HttpContext.Current != null) {
				return HttpContext.Current.Server.MapPath("~/");
			} else {
				var path = Path.GetDirectoryName(typeof(Startup).Assembly.Location);
				while (!Directory.Exists(Path.Combine(path, "ZKWeb.Owin"))) {
					path = Path.GetDirectoryName(path);
					if (string.IsNullOrEmpty(path)) {
						throw new DirectoryNotFoundException("Website root directory not found");
					}
				}
				return Path.Combine(path, "ZKWeb.Owin");
			}
		}

		/// <summary>
		/// 配置Owin程序
		/// </summary>
		/// <param name="app"></param>
		public virtual void Configuration(IAppBuilder app) {
			// 初始化程序
			var websiteRootDirectory = app.Properties.GetOrDefault<string>("host.WebsiteRootDirectory");
			websiteRootDirectory = websiteRootDirectory ?? GetWebsiteRootDirectory();
			Application.Ioc.RegisterMany<OwinWebsiteStopper>();
			Application.Initialize(websiteRootDirectory);
			// 设置处理请求的函数
			// 处理会在线程池中运行
			app.Run(owinContext => Task.Run(() => {
				var context = new OwinHttpContextWrapper(owinContext);
				try {
					// 处理请求
					Application.OnRequest(context);
				} catch (OwinHttpResponseEndException) {
					// 正常处理完毕
				} catch (Exception ex) {
					// 处理错误
					try {
						Application.OnError(context, ex);
					} catch (OwinHttpResponseEndException) {
						// 错误处理完毕
					} catch (Exception) {
						// 错误处理失败
					}
				}
			}));
		}
	}
}
