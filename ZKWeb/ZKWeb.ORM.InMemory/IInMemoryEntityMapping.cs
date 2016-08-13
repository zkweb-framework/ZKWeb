using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// The non-generic interface for entity mapping
	/// </summary>
	internal interface IInMemoryEntityMapping {
		/// <summary>
		/// The entity type
		/// </summary>
		Type EntityType { get; }
		/// <summary>
		/// Id member
		/// </summary>
		MemberInfo IdMember { get; set; }
		/// <summary>
		/// Ordinary members, not releated to other entities
		/// </summary>
		IList<MemberInfo> OrdinaryMembers { get; set; }
		/// <summary>
		/// Many-to-one members
		/// </summary>
		IList<MemberInfo> ManyToOneMembers { get; set; }
		/// <summary>
		/// One-to-many members, it should be a collection
		/// </summary>
		IList<MemberInfo> OneToManyMembers { get; set; }
		/// <summary>
		/// Many-to-many members, it should be a collection
		/// </summary>
		IList<MemberInfo> ManyToManyMembers { get; set; }
	}
}
