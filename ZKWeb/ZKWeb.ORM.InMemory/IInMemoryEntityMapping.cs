using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// The non-generic interface for entity mapping<br/>
	/// 实体映射的非泛型接口<br/>
	/// </summary>
	public interface IInMemoryEntityMapping {
		/// <summary>
		/// The entity type<br/>
		/// 实体类型<br/>
		/// </summary>
		Type EntityType { get; }
		/// <summary>
		/// Id member<br/>
		/// Id成员<br/>
		/// </summary>
		MemberInfo IdMember { get; set; }
		/// <summary>
		/// Ordinary members, not releated to other entities<br/>
		/// 普通成员, 与其他实体无关的成员<br/>
		/// </summary>
		IList<MemberInfo> OrdinaryMembers { get; set; }
		/// <summary>
		/// Many-to-one members<br/>
		/// 多对一的成员<br/>
		/// </summary>
		IList<MemberInfo> ManyToOneMembers { get; set; }
		/// <summary>
		/// One-to-many members, it should be a collection<br/>
		/// 一对多的成员, 应该是集合类型<br/>
		/// </summary>
		IList<MemberInfo> OneToManyMembers { get; set; }
		/// <summary>
		/// Many-to-many members, it should be a collection<br/>
		/// 多对多的成员, 应该是集合类型<br/>
		/// </summary>
		IList<MemberInfo> ManyToManyMembers { get; set; }
	}
}
