using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Guid utility functions<br/>
	/// <br/>
	/// </summary>
	public static class GuidUtils {
		/// <summary>
		/// Generate a sequential guid from the specific time and 8 bytes buffer<br/>
		/// The uuid version is 1, clock sequence and mac address are from the buffer<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <param name="buffer">A buffer contains atleast 8 bytes</param>
		/// <returns></returns>
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
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		public static Guid SequentialGuid(DateTime time) {
			var buffer = new byte[8];
			RandomUtils.Generator.NextBytes(buffer);
			return SequentialGuid(time, buffer);
		}

		/// <summary>
		/// Generate a sequential guid from the specific time<br/>
		/// It use secure random bytes for clock sequence and mac address<br/>
		/// usually use to generate a more secure guid like session id<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		public static Guid SecureSequentialGuid(DateTime time) {
			return SequentialGuid(time, RandomUtils.SystemRandomBytes(8));
		}
	}
}
