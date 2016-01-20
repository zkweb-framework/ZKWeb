using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 密码工具类
	/// </summary>
	public static class PasswordUtils {
		/// <summary>
		/// 获取指定数据的PBKDF2校验值
		/// </summary>
		/// <param name="data"></param>
		/// <param name="slat">参与校验的随机数据，长度需要等于8</param>
		/// <param name="iterations">计算循环次数，越长强度越高但越耗费性能</param>
		/// <param name="hashLength">校验值长度</param>
		/// <returns></returns>
		public static byte[] PBKDF2Sum(
			byte[] data, byte[] slat, int iterations = 1024, int hashLength = 32) {
			var hash = new Rfc2898DeriveBytes(data, slat, iterations).GetBytes(hashLength);
			return hash;
		}

		/// <summary>
		/// 获取指定数据的Md5校验值
		/// </summary>
		public static byte[] Md5Sum(byte[] data) {
			return MD5.Create().ComputeHash(data);
		}

		/// <summary>
		/// 获取指定数据的Sha1校验值
		/// </summary>
		public static byte[] Sha1Sum(byte[] data) {
			return SHA1.Create().ComputeHash(data);
		}
	}

	/// <summary>
	/// 密码信息
	/// </summary>
	public class PasswordInfo {
		/// <summary>
		/// 密码类型
		/// </summary>
		public PasswordHashType Type { get; set; } = PasswordHashType.PBKDF2;
		/// <summary>
		/// 参与校验的随机数据（base64）
		/// </summary>
		public string Slat { get; set; }
		/// <summary>
		/// 密码校验值（base64）
		/// </summary>
		public string Hash { get; set; }

		/// <summary>
		/// 检查密码，返回密码是否正确
		/// </summary>
		/// <param name="password"></param>
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
		/// 从密码创建密码信息
		/// </summary>
		/// <param name="password">密码</param>
		/// <param name="slat">参与校验的随机数据，不指定时使用默认值</param>
		/// <param name="type">密码类型</param>
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
		/// 默认，等于PBKDF2
		/// </summary>
		Default = PBKDF2,
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
