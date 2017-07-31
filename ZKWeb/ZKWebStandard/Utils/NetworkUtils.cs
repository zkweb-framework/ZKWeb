using System.Net;
using System.Net.Sockets;
using ZKWebStandard.Web;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Network utility functions<br/>
	/// 网络的工具函数<br/>
	/// </summary>
	public static class NetworkUtils {
		/// <summary>
		/// Get client ip address<br/>
		/// If there a http request then return the remote address<br/>
		/// otherwise return host ip address<br/>
		/// 获取客户端的IP地址<br/>
		/// 如果当前有Http连接则返回远程的IP地址<br/>
		/// 否则返回本机地址<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var address = NetworkUtils.GetClientIpAddress();
		/// </code>
		/// </example>
		public static IPAddress GetClientIpAddress() {
			if (HttpManager.CurrentContextExists) {
				return HttpManager.CurrentContext.Request.RemoteIpAddress;
			} else {
				return GetHostIpAddress();
			}
		}

		/// <summary>
		/// Get host ip address<br/>
		/// Notice if host is in LAN then this function will return lan ip not public ip address<br/>
		/// 获取本机IP地址<br/>
		/// 注意如果本机在局域网中这个函数会返回局域网IP, 而不是公网IP<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var address = NetworkUtils.GetHostIpAddress();
		/// </code>
		/// </example>
		public static IPAddress GetHostIpAddress() {
			using (var socket =
					new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				try {
					socket.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
					return ((IPEndPoint)socket.LocalEndPoint).Address;
				} catch (SocketException) {
					return IPAddress.Loopback;
				}
			}
		}
	}
}
