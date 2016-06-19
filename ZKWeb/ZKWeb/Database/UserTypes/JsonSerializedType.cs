using Newtonsoft.Json;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;

namespace ZKWeb.Database.UserTypes {
	/// <summary>
	/// 使用Json序列化到数据库的成员类型
	/// null处理: 反序列化成员时如果结果等于null会自动新建
	/// </summary>
	/// <example>
	/// Map(s => s.Items).CustomType[JsonSerializedType[Dictionary[string, object]]]();
	/// </example>
	/// <typeparam name="T"></typeparam>
	public class JsonSerializedType<T> : IUserType
		where T : class, new() {
		/// <summary>
		/// 是否可修改
		/// </summary>
		public bool IsMutable {
			get { return false; }
		}

		/// <summary>
		/// NullSafeGet的返回类型
		/// </summary>
		public Type ReturnedType {
			get { return typeof(T); }
		}

		/// <summary>
		/// Sql类型
		/// </summary>
		public SqlType[] SqlTypes {
			get { return new SqlType[] { new SqlType(DbType.String, 0xffff) }; }
		}

		/// <summary>
		/// 从缓存读取时的转换
		/// </summary>
		public object Assemble(object cached, object owner) {
			return cached;
		}

		/// <summary>
		/// 复制对象
		/// </summary>
		public object DeepCopy(object value) {
			return value;
		}

		/// <summary>
		/// 保存到缓存时的转换
		/// </summary>
		public object Disassemble(object value) {
			return value;
		}

		/// <summary>
		/// 判断是否相等
		/// </summary>
		public new bool Equals(object x, object y) {
			return object.Equals(x, y);
		}

		/// <summary>
		/// 获取hash值
		/// </summary>
		public int GetHashCode(object x) {
			return x.GetHashCode();
		}

		/// <summary>
		/// 从已序列化中的数据获取成员的值
		/// </summary>
		public object NullSafeGet(IDataReader rs, string[] names, object owner) {
			var json = NHibernateUtil.String.NullSafeGet(rs, names[0]) as string;
			if (string.IsNullOrEmpty(json)) {
				return new T();
			}
			return JsonConvert.DeserializeObject<T>(json) ?? new T();
		}

		/// <summary>
		/// 序列化成员的值到数据
		/// </summary>
		public void NullSafeSet(IDbCommand cmd, object value, int index) {
			var json = JsonConvert.SerializeObject(value);
			((IDataParameter)cmd.Parameters[index]).Value = json;
		}

		/// <summary>
		/// 合并对象替换target到original时的处理
		/// </summary>
		public object Replace(object original, object target, object owner) {
			return original;
		}
	}
}
