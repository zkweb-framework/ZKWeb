using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// Interface for MongoDB entity mapping<br/>
	/// MongoDB的实体映射接口<br/>
	/// </summary>
	public interface IMongoDBEntityMapping {
		/// <summary>
		/// Collection name<br/>
		/// 集合名词<br/>
		/// </summary>
		string CollectionName { get; }
		/// <summary>
		/// Id member<br/>
		/// Id成员<br/>
		/// </summary>
		MemberInfo IdMember { get; }
		/// <summary>
		/// Ordinary members, not releated to other entities<br/>
		/// 普通成员, 和其他实体无关的成员<br/>
		/// </summary>
		IEnumerable<MemberInfo> OrdinaryMembers { get; }
	}
}
