using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// The non-generic interface for entity mapping<br/>
	/// 非泛型的实体映射定义的接口<br/>
	/// </summary>
	public interface IDapperEntityMapping {
		/// <summary>
		/// The entity type<br/>
		/// 实体类型<br/>
		/// </summary>
		Type EntityType { get; }
		/// <summary>
		/// The table name<br/>
		/// 表名<br/>
		/// </summary>
		string TableName { get; }
		/// <summary>
		/// Id member<br/>
		/// Id成员<br/>
		/// </summary>
		MemberInfo IdMember { get; }
		/// <summary>
		/// Ordinary members, not releated to other entities<br/>
		/// 普通成员, 与其他实体无关的成员<br/>
		/// </summary>
		IEnumerable<MemberInfo> OrdinaryMembers { get; }
	}
}
