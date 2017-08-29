using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;
using ZKWeb.Logging;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB entity mapping builder<br/>
	/// Attention: The entity type can only be mapped once, the following mapping configuration will be ignored<br/>
	/// MongoDB的实体映射构建器<br/>
	/// 注意: 各个实体类型全局只能映射一次, 后面的映射会被忽略<br/>
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class MongoDBEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>,
		IMongoDBEntityMapping
		where T : class, IEntity {
		/// <summary>
		/// Actions perform to bson class mapping<br/>
		/// 应用到Bson类型映射的操作<br/>
		/// </summary>
		protected IList<Action<BsonClassMap<T>>> MapActions { get; set; }
		/// <summary>
		/// Actions perform to data collection<br/>
		/// 应用到数据集合的操作<br/>
		/// </summary>
		protected IList<Action<IMongoCollection<T>>> CollectionActions { get; set; }
		/// <summary>
		/// Collection name<br/>
		/// 数据集合名词<br/>
		/// </summary>
		public string CollectionName { get { return collectionName; } }
#pragma warning disable CS1591
		protected string collectionName;
#pragma warning restore CS1591
		/// <summary>
		/// Id member<br/>
		/// Id成员<br/>
		/// </summary>
		public MemberInfo IdMember { get { return idMember; } }
#pragma warning disable CS1591
		protected MemberInfo idMember;
#pragma warning restore CS1591
		/// <summary>
		/// Ordinary members<br/>
		/// 普通成员<br/>
		/// </summary>
		public IEnumerable<MemberInfo> OrdinaryMembers { get { return ordinaryMembers; } }
#pragma warning disable CS1591
		protected IList<MemberInfo> ordinaryMembers;
#pragma warning restore CS1591
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return MongoDBDatabaseContext.ConstORM; } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public MongoDBEntityMappingBuilder(
			IMongoDatabase database,
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			MapActions = new List<Action<BsonClassMap<T>>>();
			CollectionActions = new List<Action<IMongoCollection<T>>>();
			collectionName = typeof(T).Name;
			idMember = null;
			ordinaryMembers = new List<MemberInfo>();
			// Configure with registered provider
			foreach (IEntityMappingProvider<T> provider in providers) {
				provider.Configure(this);
			}
			// Register mapping
			if (!BsonClassMap.IsClassMapRegistered(typeof(T))) {
				BsonClassMap.RegisterClassMap<T>(m => {
					MapActions.ForEach(a => a(m));
					m.SetIgnoreExtraElements(true);
				});
			}
			// Convert collection name with registered hanlders
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
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		public void TableName(string tableName) {
			collectionName = tableName;
		}

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// 指定实体的主键<br/>
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
		/// Create a member mapping<br/>
		/// 创建成员映射<br/>
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
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"References is unsupported with mongodb, expression: {memberExpression}");
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"HasMany is unsupported with mongodb, expression: {memberExpression}");
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"HasManyToMany is unsupported with mongodb, expression: {memberExpression}");
		}
	}
}
