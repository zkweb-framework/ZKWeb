using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// The non-generic interface for entity mapping<br/>
	/// <br/>
	/// </summary>
	internal interface IDapperEntityMapping {
		/// <summary>
		/// The entity type<br/>
		/// <br/>
		/// </summary>
		Type EntityType { get; }
		/// <summary>
		/// The table name<br/>
		/// <br/>
		/// </summary>
		string TableName { get; }
		/// <summary>
		/// Id member<br/>
		/// <br/>
		/// </summary>
		MemberInfo IdMember { get; }
		/// <summary>
		/// Ordinary members, not releated to other entities<br/>
		/// <br/>
		/// </summary>
		IEnumerable<MemberInfo> OrdinaryMembers { get; }
		/// <summary>
		/// Ignore members that not mapped by this builder<br/>
		/// <br/>
		/// </summary>
		void IgnoreExtraMembers();
	}
}
