using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Guid utility functions<br/>
	/// Guid的工具函数<br/>
	/// </summary>
	public static class GuidUtils {
		/// <summary>
		/// Generate a sequential guid from the specific time and 8 bytes buffer<br/>
		/// The uuid version is 1, clock sequence and mac address are from the buffer<br/>
		/// 根据8个字节的缓冲区和指定时间生成序列Guid<br/>
		/// UUID Version是1, 时钟序号和MAC地址由缓冲区得到<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <param name="buffer">A buffer contains atleast 8 bytes</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var guid = GuidUtils.SequentialGuid(new DateTime(2016, 8, 8), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
		/// </code>
		/// </example>
		private static Guid SequentialGuid(DateTime time, byte[] buffer) {
			var ticks = (time - new DateTime(1900, 1, 1)).Ticks;
			var guid = new Guid(
				(uint)(ticks >> 32),
				(ushort)(ticks >> 16),
				(ushort)((ticks & 0x3f) | (1 << 12)),
				buffer[0], buffer[1], buffer[2], buffer[3],
				buffer[4], buffer[5], buffer[6], buffer[7]);
			return guid;
		}

		/// <summary>
		/// Generate a sequential guid from the specific time<br/>
		/// It use random bytes for clock sequence and mac address<br/>
		/// 根据指定时间生成序列GUID<br/>
		/// 它使用了随机值代替时钟序列和MAC地址<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var guid = GuidUtils.SequentialGuid(new DateTime(2016, 8, 8));
		/// </code>
		/// </example>
		public static Guid SequentialGuid(DateTime time) {
			var buffer = new byte[8];
			RandomUtils.Generator.NextBytes(buffer);
			return SequentialGuid(time, buffer);
		}

		/// <summary>
		/// Generate a sequential guid from the specific time<br/>
		/// It use secure random bytes for clock sequence and mac address<br/>
		/// usually use to generate a more secure guid like session id<br/>
		/// 根据指定时间生成序列GUID<br/>
		/// 它使用了安全的随机值代替时钟序列和MAC地址<br/>
		/// 通常用于生成更安全的Guid值, 例如会话Id<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var guid = GuidUtils.SecureSequentialGuid(new DateTime(2016, 8, 8)); 
		/// </code>
		/// </example>
		public static Guid SecureSequentialGuid(DateTime time) {
			return SequentialGuid(time, RandomUtils.SystemRandomBytes(8));
		}
	}
}
