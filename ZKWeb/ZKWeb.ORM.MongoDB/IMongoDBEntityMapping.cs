using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// Interface for mongodb entity mapping<br/>
	/// <br/>
	/// </summary>
	internal interface IMongoDBEntityMapping {
		/// <summary>
		/// Collection name<br/>
		/// <br/>
		/// </summary>
		string CollectionName { get; }
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
	}
}
