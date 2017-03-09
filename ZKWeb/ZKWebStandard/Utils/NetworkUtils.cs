using System.Net;
using System.Net.Sockets;
using ZKWebStandard.Web;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Network utility functions
	/// </summary>
	public static class NetworkUtils {
		/// <summary>
		/// Get client ip address
		/// If there a http request then return the remote address
		/// otherwise return host ip address
		/// </summary>
		/// <returns></returns>
		public static IPAddress GetClientIpAddress() {
			if (HttpManager.CurrentContextExists) {
				return HttpManager.CurrentContext.Request.RemoteIpAddress;
			} else {
				return GetHostIpAddress();
			}
		}

		/// <summary>
		/// Get host ip address
		/// Notice if host is in LAN then this function will return lan ip not public ip address
		/// </summary>
		/// <returns></returns>
		public static IPAddress GetHostIpAddress() {
			using (var socket =
					new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				socket.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
				return ((IPEndPoint)socket.LocalEndPoint).Address;
			}
		}
	}
}
