using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// Interface for mongodb entity mapping
	/// </summary>
	internal interface IMongoDBEntityMapping {
		/// <summary>
		/// Collection name
		/// </summary>
		string CollectionName { get; }
		/// <summary>
		/// Id member
		/// </summary>
		MemberInfo IdMember { get; }
		/// <summary>
		/// Ordinary members, not releated to other entities
		/// </summary>
		IEnumerable<MemberInfo> OrdinaryMembers { get; }
	}
}
