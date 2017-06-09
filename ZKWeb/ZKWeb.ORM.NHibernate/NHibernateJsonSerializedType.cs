using Newtonsoft.Json;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Use json serialize before store into database and<br/>
	/// use json deserialize after retrieve from database.<br/>
	/// It value retrieve from database is null, it will create a new object instead.<br/>
	/// 保存到数据库前使用Json序列化<br/>
	/// 从数据库获取后使用Json反序列化<br/>
	/// 如果从数据库得到的值是null, 它会new一个代替<br/>
	/// </summary>
	/// <typeparam name="T">Member type</typeparam>
	public class NHibernateJsonSerializedType<T> :
		IUserType
		where T : class, new() {
		/// <summary>
		/// Is mutable<br/>
		/// 数据是否可变<br/>
		/// </summary>
		public bool IsMutable {
			get { return false; }
		}

		/// <summary>
		/// The return type of NullSafeGet<br/>
		/// NullSafeGet的返回类型<br/>
		/// </summary>
		public Type ReturnedType {
			get { return typeof(T); }
		}

		/// <summary>
		/// The sql type<br/>
		/// SQL类型<br/>
		/// </summary>
		public SqlType[] SqlTypes {
			get { return new SqlType[] { new SqlType(DbType.String, 0xffff) }; }
		}

		/// <summary>
		/// Assemble after loaded from cache<br/>
		/// 从缓存取出后的处理<br/>
		/// </summary>
		public object Assemble(object cached, object owner) {
			return cached;
		}

		/// <summary>
		/// Deep copy the value<br/>
		/// 深度复制值<br/>
		/// </summary>
		public object DeepCopy(object value) {
			return value;
		}

		/// <summary>
		/// Disassemble before store to cache<br/>
		/// 保存到缓存前的处理<br/>
		/// </summary>
		public object Disassemble(object value) {
			return value;
		}

		/// <summary>
		/// Check it's equals<br/>
		/// 检查是否相等<br/>
		/// </summary>
		public new bool Equals(object x, object y) {
			return object.Equals(x, y);
		}

		/// <summary>
		/// Get hash code<br/>
		/// 获取校验码<br/>
		/// </summary>
		public int GetHashCode(object x) {
			return x.GetHashCode();
		}

		/// <summary>
		/// Deserialize value after retrieve from database,<br/>
		/// if it's null then return a new object<br/>
		/// 从数据库获取后反序列化值<br/>
		/// 如果等于null则返回一个新对象<br/>
		/// </summary>
		public object NullSafeGet(IDataReader rs, string[] names, object owner) {
			var json = NHibernateUtil.String.NullSafeGet(rs, names[0]) as string;
			if (string.IsNullOrEmpty(json)) {
				return new T();
			}
			return JsonConvert.DeserializeObject<T>(json) ?? new T();
		}

		/// <summary>
		/// Serialize value before store into database<br/>
		/// 保存到数据库前序列化值<br/>
		/// </summary>
		public void NullSafeSet(IDbCommand cmd, object value, int index) {
			var json = JsonConvert.SerializeObject(value);
			((IDataParameter)cmd.Parameters[index]).Value = json;
		}

		/// <summary>
		/// The behavior of replace member value, just ignore the exist value<br/>
		/// 替换成员值时的处理, 直接忽略原值<br/>
		/// </summary>
		public object Replace(object original, object target, object owner) {
			return original;
		}
	}
}
