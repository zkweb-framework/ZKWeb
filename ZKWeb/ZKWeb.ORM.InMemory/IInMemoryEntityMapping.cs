using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// The non-generic interface for entity mapping<br/>
	/// <br/>
	/// </summary>
	internal interface IInMemoryEntityMapping {
		/// <summary>
		/// The entity type<br/>
		/// <br/>
		/// </summary>
		Type EntityType { get; }
		/// <summary>
		/// Id member<br/>
		/// <br/>
		/// </summary>
		MemberInfo IdMember { get; set; }
		/// <summary>
		/// Ordinary members, not releated to other entities<br/>
		/// <br/>
		/// </summary>
		IList<MemberInfo> OrdinaryMembers { get; set; }
		/// <summary>
		/// Many-to-one members<br/>
		/// <br/>
		/// </summary>
		IList<MemberInfo> ManyToOneMembers { get; set; }
		/// <summary>
		/// One-to-many members, it should be a collection<br/>
		/// <br/>
		/// </summary>
		IList<MemberInfo> OneToManyMembers { get; set; }
		/// <summary>
		/// Many-to-many members, it should be a collection<br/>
		/// <br/>
		/// </summary>
		IList<MemberInfo> ManyToManyMembers { get; set; }
	}
}
