using System;
using FluentNHibernate.Mapping;
using ZKWeb.Database;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Defines a mapping for an entity<br/>
	/// 定义指定类型的映射<br/>
	/// See: http://stackoverflow.com/questions/1152060/nhibernate-cascade-save-update
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class NHibernateEntityMappingBuilder<T> :
		ClassMap<T>,
		IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return NHibernateDatabaseContext.ConstORM; } }
		/// <summary>
		/// Custom table name<br/>
		/// 自定义表名<br/>
		/// </summary>
		protected string CustomTableName { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public NHibernateEntityMappingBuilder() {
			// Configure with registered providers
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
			// Set table name with registered handlers
			var tableName = CustomTableName ?? typeof(T).Name;
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			foreach (var handler in handlers) {
				handler.ConvertTableName(ref tableName);
			}
			base.Table(tableName);
		}

		/// <summary>
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		/// <param name="tableName">The table name</param>
		public void TableName(string tableName) {
			CustomTableName = tableName;
		}

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// 指定实体的主键<br/>
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			// Unsupported options: Unique, Nullable, Index, CascadeDelete
			options = options ?? new EntityMappingOptions();
			var idPart = base.Id(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			if (!string.IsNullOrEmpty(options.Column)) {
				idPart = idPart.Column(options.Column);
			}
			if (options.Length != null) {
				idPart = idPart.Length(checked((int)options.Length.Value));
			}
			if (!string.IsNullOrEmpty(options.CustomSqlType)) {
				idPart = idPart.CustomSqlType(options.CustomSqlType);
			}
			if (options.WithSerialization == true) {
				idPart = idPart.CustomType(
					typeof(NHibernateJsonSerializedType<>).MakeGenericType(typeof(TPrimaryKey)));
			}
		}

		/// <summary>
		/// Create a member mapping<br/>
		/// 创建成员映射<br/>
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options = null) {
			// Unsupported options: CascadeDelete
			options = options ?? new EntityMappingOptions();
			var memberPart = base.Map(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			var property = (PropertyInfo)((MemberExpression)memberExpression.Body).Member;
			if (!string.IsNullOrEmpty(options.Column)) {
				memberPart = memberPart.Column(options.Column);
			}
			if (options.Length != null) {
				memberPart = memberPart.Length(checked((int)options.Length.Value));
			} else if (property.PropertyType == typeof(string)) {
				memberPart = memberPart.Length(0xffff); // set max length for string by default
			}
			if (options.Unique == true) {
				memberPart = memberPart.Unique();
			}
			if (options.Nullable == true) {
				memberPart = memberPart.Nullable();
			} else if (options.Nullable == false) {
				memberPart = memberPart.Not.Nullable();
			}
			if (!string.IsNullOrEmpty(options.Index)) {
				memberPart = memberPart.Index(options.Index);
			}
			if (!string.IsNullOrEmpty(options.CustomSqlType)) {
				memberPart = memberPart.CustomSqlType(options.CustomSqlType);
			} else if (property.PropertyType.IsEnum) {
				memberPart = memberPart.CustomType<int>(); // store enum as int type by default
			}
			if (options.WithSerialization == true) {
				memberPart = memberPart.CustomType(
					typeof(NHibernateJsonSerializedType<>).MakeGenericType(typeof(TMember)));
			}
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options = null)
			where TOther : class {
			// Unsupported options: Length, Unique, Index,
			// CustomSqlType, WithSerialization, Navigation
			options = options ?? new EntityMappingOptions();
			var manyToOnePart = base.References(memberExpression);
			if (!string.IsNullOrEmpty(options.Column)) {
				manyToOnePart = manyToOnePart.Column(options.Column);
			}
			if (options.Nullable == true) {
				manyToOnePart = manyToOnePart.Nullable();
			} else if (options.Nullable == false) {
				manyToOnePart = manyToOnePart.Not.Nullable();
			}
			// Cascade should specified on parent side, but just support this option
			if (options.CascadeDelete == false) {
				manyToOnePart.Cascade.None();
			} else if (options.CascadeDelete == true) {
				manyToOnePart.Cascade.DeleteOrphans();
			}
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			// Unsupported options: Column, Length, Unique,
			// Nullable, Index, CustomSqlType, WithSerialization
			options = options ?? new EntityMappingOptions();
			var oneToManyPart = base.HasMany(memberExpression);
			if (!string.IsNullOrEmpty(options.Navigation)) {
				oneToManyPart = oneToManyPart.KeyColumn(options.Navigation);
			}
			if (options.CascadeDelete == false) {
				oneToManyPart.Cascade.None();
			} else {
				oneToManyPart.Cascade.AllDeleteOrphan(); // true or default
			}
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			// Unsupported options: Column, Length, Unique,
			// Nullable, Index, CustomSqlType, WithSerialization
			options = options ?? new EntityMappingOptions();
			var manyToManyPart = base.HasManyToMany(memberExpression);
			if (!string.IsNullOrEmpty(options.Navigation)) {
				manyToManyPart.ChildKeyColumn(options.Navigation);
			}
			if (options.CascadeDelete == true) {
				manyToManyPart.Cascade.AllDeleteOrphan();
			} else {
				manyToManyPart.Cascade.None(); // false or default
			}
		}
	}
}
