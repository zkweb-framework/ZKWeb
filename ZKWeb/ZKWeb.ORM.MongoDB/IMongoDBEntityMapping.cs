using MongoDB.Driver;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// Interface for mongodb entity mapping
	/// </summary>
	internal interface IMongoDBEntityMapping {
		/// <summary>
		/// Collection name
		/// </summary>
		string CollectionName { get; }
	}
}
