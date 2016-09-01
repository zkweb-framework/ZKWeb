using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB entity mapping builder
	/// Attention: The entity type can only be mapped once, the following mapping configuration will be ignored
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	internal class MongoDBEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>, IMongoDBEntityMapping
		where T : class, IEntity {
		/// <summary>
		/// Actions perform to bson class mapping
		/// </summary>
		private IList<Action<BsonClassMap<T>>> MapActions { get; set; }
		/// <summary>
		/// Actions perform to data collection
		/// </summary>
		private IList<Action<IMongoCollection<T>>> CollectionActions { get; set; }
		/// <summary>
		/// Collection name
		/// </summary>
		public string CollectionName { get { return collectionName; } }
		private string collectionName;
		/// <summary>
		/// Id member
		/// </summary>
		public MemberInfo IdMember { get { return idMember; } }
		private MemberInfo idMember;
		/// <summary>
		/// Ordinary members
		/// </summary>
		public IEnumerable<MemberInfo> OrdinaryMembers { get { return ordinaryMembers; } }
		private IList<MemberInfo> ordinaryMembers;
		/// <summary>
		/// ORM name
		/// </summary>
		public string ORM { get { return MongoDBDatabaseContext.ConstORM; } }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database object</param>
		public MongoDBEntityMappingBuilder(IMongoDatabase database) {
			MapActions = new List<Action<BsonClassMap<T>>>();
			CollectionActions = new List<Action<IMongoCollection<T>>>();
			collectionName = typeof(T).Name;
			idMember = null;
			ordinaryMembers = new List<MemberInfo>();
			// Configure with registered provider
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
			// Register mapping
			BsonClassMap.RegisterClassMap<T>(m => {
				MapActions.ForEach(a => a(m));
				m.SetIgnoreExtraElements(true);
			});
			// Get collection name with registered hanlders
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			foreach (var handler in handlers) {
				handler.ConvertTableName(ref collectionName);
			}
			// Create indexes
			var collection = database.GetCollection<T>(collectionName);
			CollectionActions.ForEach(a => {
				a(collection);
			});
		}

		/// <summary>
		/// Specify the primary key for this entity
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			// Unsupported options: Length, Unique, Nullable
			// Index, CustomSqlType, CascadeDelete, WithSerialization
			options = options ?? new EntityMappingOptions();
			idMember = ((MemberExpression)memberExpression.Body).Member;
			MapActions.Add(m => {
				var memberMap = m.MapIdMember(memberExpression);
				if (!string.IsNullOrEmpty(options.Column)) {
					memberMap = memberMap.SetElementName(options.Column);
				}
			});
		}

		/// <summary>
		/// Create a member mapping
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			// Unsupported options: Length, CustomSqlType, CascadeDelete, WithSerialization
			options = options ?? new EntityMappingOptions();
			ordinaryMembers.Add(((MemberExpression)memberExpression.Body).Member);
			MapActions.Add(m => {
				var memberMap = m.MapMember(memberExpression);
				if (!string.IsNullOrEmpty(options.Column)) {
					memberMap = memberMap.SetElementName(options.Column);
				}
				if (options.Nullable == true) {
					memberMap = memberMap.SetIsRequired(true);
				} else if (options.Nullable == false) {
					memberMap = memberMap.SetIsRequired(false);
				}
			});
			if (options.Unique == true || !string.IsNullOrEmpty(options.Index)) {
				// Create indexes
				CollectionActions.Add(c => {
					var keys = new IndexKeysDefinitionBuilder<T>().Ascending(
						Expression.Lambda<Func<T, object>>(
							Expression.Convert(memberExpression.Body, typeof(object)),
							memberExpression.Parameters));
					var indxOptions = new CreateIndexOptions() {
						Background = true,
						Unique = options.Unique,
						Sparse = !options.Unique // ignore null member on indexing
					};
					c.Indexes.CreateOne(keys, indxOptions);
				});
			}
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			throw new NotSupportedException("References is not supported with MongoDB");
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			throw new NotSupportedException("HasMany is not supported with MongoDB");
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			throw new NotSupportedException("HasManyToMany is not supported with MongoDB");
		}
	}
}
