using System;
using System.Security.Cryptography;
using System.Text;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Password utility functions
	/// </summary>
	public static class PasswordUtils {
		/// <summary>
		/// Get PBKDF2 checksum
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="slat">The slat, length should be 8</param>
		/// <param name="iterations">Iteration times</param>
		/// <param name="hashLength">Hash length</param>
		/// <returns></returns>
		public static byte[] PBKDF2Sum(
			byte[] data, byte[] slat, int iterations = 1024, int hashLength = 32) {
			var hash = new Rfc2898DeriveBytes(data, slat, iterations).GetBytes(hashLength);
			return hash;
		}

		/// <summary>
		/// Get md5 checksum
		/// </summary>
		public static byte[] Md5Sum(byte[] data) {
			return MD5.Create().ComputeHash(data);
		}

		/// <summary>
		/// Get sha1 checksum
		/// </summary>
		public static byte[] Sha1Sum(byte[] data) {
			return SHA1.Create().ComputeHash(data);
		}
	}

	/// <summary>
	/// Password information
	/// </summary>
	public class PasswordInfo {
		/// <summary>
		/// Password type
		/// </summary>
		public PasswordHashType Type { get; set; } = PasswordHashType.PBKDF2;
		/// <summary>
		/// Slat in base64
		/// </summary>
		public string Slat { get; set; }
		/// <summary>
		/// Hash in base64
		/// </summary>
		public string Hash { get; set; }

		/// <summary>
		/// Check password, return true for success
		/// </summary>
		/// <param name="password">The password</param>
		/// <returns></returns>
		public bool Check(string password) {
			if (string.IsNullOrEmpty(password)) {
				return false;
			}
			var slat = Slat == null ? null : Convert.FromBase64String(Slat);
			var info = FromPassword(password, slat, Type);
			return (Hash == info.Hash);
		}

		/// <summary>
		/// Create password information from password
		/// </summary>
		/// <param name="password">The password</param>
		/// <param name="slat">Slat, use a random value if gived null</param>
		/// <param name="type">Password type</param>
		public static PasswordInfo FromPassword(string password,
			byte[] slat = null, PasswordHashType type = PasswordHashType.PBKDF2) {
			if (string.IsNullOrEmpty(password)) {
				throw new ArgumentNullException("password can't be empty");
			}
			var info = new PasswordInfo() { Type = type };
			var passwordBytes = Encoding.UTF8.GetBytes(password);
			if (type == PasswordHashType.PBKDF2) {
				slat = slat ?? RandomUtils.SystemRandomBytes(8);
				info.Slat = Convert.ToBase64String(slat);
				info.Hash = Convert.ToBase64String(PasswordUtils.PBKDF2Sum(passwordBytes, slat));
			} else if (type == PasswordHashType.Md5) {
				info.Hash = Convert.ToBase64String(PasswordUtils.Md5Sum(passwordBytes));
			} else if (type == PasswordHashType.Sha1) {
				info.Hash = Convert.ToBase64String(PasswordUtils.Sha1Sum(passwordBytes));
			}
			return info;
		}
	}

	/// <summary>
	/// 密码类型
	/// </summary>
	public enum PasswordHashType {
		/// <summary>
		/// PBKDF2
		/// </summary>
		PBKDF2 = 0,
		/// <summary>
		/// Md5
		/// </summary>
		Md5 = 1,
		/// <summary>
		/// Sha1
		/// </summary>
		Sha1 = 2
	}
}
