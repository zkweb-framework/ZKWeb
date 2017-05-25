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
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="T">Member type</typeparam>
	public class NHibernateJsonSerializedType<T> : IUserType
		where T : class, new() {
		/// <summary>
		/// Is mutable<br/>
		/// <br/>
		/// </summary>
		public bool IsMutable {
			get { return false; }
		}

		/// <summary>
		/// The return type of NullSafeGet<br/>
		/// <br/>
		/// </summary>
		public Type ReturnedType {
			get { return typeof(T); }
		}

		/// <summary>
		/// The sql type<br/>
		/// <br/>
		/// </summary>
		public SqlType[] SqlTypes {
			get { return new SqlType[] { new SqlType(DbType.String, 0xffff) }; }
		}

		/// <summary>
		/// Assemble after loaded from cache<br/>
		/// <br/>
		/// </summary>
		public object Assemble(object cached, object owner) {
			return cached;
		}

		/// <summary>
		/// Deep copy the value<br/>
		/// <br/>
		/// </summary>
		public object DeepCopy(object value) {
			return value;
		}

		/// <summary>
		/// Disassemble before store to cache<br/>
		/// <br/>
		/// </summary>
		public object Disassemble(object value) {
			return value;
		}

		/// <summary>
		/// Check it's equals<br/>
		/// <br/>
		/// </summary>
		public new bool Equals(object x, object y) {
			return object.Equals(x, y);
		}

		/// <summary>
		/// Get hash code<br/>
		/// <br/>
		/// </summary>
		public int GetHashCode(object x) {
			return x.GetHashCode();
		}

		/// <summary>
		/// Deserialize value after retrieve from database,<br/>
		/// if it's null then return a new object<br/>
		/// <br/>
		/// <br/>
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
		/// <br/>
		/// </summary>
		public void NullSafeSet(IDbCommand cmd, object value, int index) {
			var json = JsonConvert.SerializeObject(value);
			((IDataParameter)cmd.Parameters[index]).Value = json;
		}

		/// <summary>
		/// The behavior of replace member value, just ignore the exist value<br/>
		/// <br/>
		/// </summary>
		public object Replace(object original, object target, object owner) {
			return original;
		}
	}
}
