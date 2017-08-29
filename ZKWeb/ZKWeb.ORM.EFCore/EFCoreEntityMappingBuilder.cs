using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using ZKWeb.Database;
using ZKWeb.Logging;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core entity mapping builder<br/>
	/// Entity Framework Core的实体映射构建器<br/>
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class EFCoreEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// Entity Framework's native builder<br/>
		/// 原生的EF实体映射构建器<br/>
		/// </summary>
		public EntityTypeBuilder<T> Builder { get; protected set; }
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return EFCoreDatabaseContext.ConstORM; } }
		/// <summary>
		/// Custom table name<br/>
		/// 自定义表名<br/>
		/// </summary>
		protected string CustomTableName { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public EFCoreEntityMappingBuilder(
			ModelBuilder builder,
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			Builder = builder.Entity<T>();
			// Configure with registered providers
			foreach (IEntityMappingProvider<T> provider in providers) {
				provider.Configure(this);
			}
			// Set table name with registered handlers
			var tableName = CustomTableName ?? typeof(T).Name;
			foreach (var handler in handlers) {
				handler.ConvertTableName(ref tableName);
			}
			Builder = Builder.ToTable(tableName);
		}

		/// <summary>
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		/// <param name="tableName"></param>
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
			// Unsupported options: Length, Unique, Nullable,
			// Index, CustomSqlType, CascadeDelete, WithSerialization
			options = options ?? new EntityMappingOptions();
			var keyBuilder = Builder.HasKey(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			if (!string.IsNullOrEmpty(options.Column)) {
				keyBuilder = keyBuilder.HasName(options.Column);
			}
		}

		/// <summary>
		/// Create a member mapping<br/>
		/// 创建成员映射<br/>
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			// Unsupported options: CascadeDelete
			options = options ?? new EntityMappingOptions();
			var propertyBuilder = Builder.Property(memberExpression);
			if (!string.IsNullOrEmpty(options.Column)) {
				propertyBuilder = propertyBuilder.HasColumnName(options.Column);
			}
			if (options.Length != null) {
				propertyBuilder = propertyBuilder.HasMaxLength(
					checked((int)options.Length.Value));
			}
			if (options.Unique == true) {
				// See http://stackoverflow.com/questions/35309553/the-property-on-entity-type-is-part-of-a-key-and-so-cannot-be-modified-or-marked
				Builder.HasIndex(Expression.Lambda<Func<T, object>>(
					Expression.Convert(memberExpression.Body, typeof(object)),
					memberExpression.Parameters)).IsUnique();
			}
			if (options.Nullable == true) {
				propertyBuilder = propertyBuilder.IsRequired(false);
			} else if (options.Nullable == false) {
				propertyBuilder = propertyBuilder.IsRequired(true);
			}
			if (!string.IsNullOrEmpty(options.Index)) {
				Builder.HasIndex(Expression.Lambda<Func<T, object>>(
					Expression.Convert(memberExpression.Body, typeof(object)),
					memberExpression.Parameters)).HasName(options.Index);
			}
			if (!string.IsNullOrEmpty(options.CustomSqlType)) {
				propertyBuilder = propertyBuilder.HasColumnType(options.CustomSqlType);
			}
			if (options.WithSerialization == true) {
				// log error only, some functions may not work
				var logManager = Application.Ioc.Resolve<LogManager>();
				logManager.LogError(
					"Entity framework core not support custom type mapping yet, " +
					"see https://github.com/aspnet/EntityFramework/issues/242 " +
					$"expression: {memberExpression}");
			}
		}

		/// <summary>
		/// Use navigation property name from options, or<br/>
		/// Automatic determine navigation property name on the other side<br/>
		/// 从选项获取导航属性名称<br/>
		/// 或自动检测另一端的导航属性<br/>
		/// </summary>
		protected string GetNavigationPropertyName<TOther, TNavigationType>(
			EntityMappingOptions options) {
			if (!string.IsNullOrEmpty(options.Navigation)) {
				return options.Navigation;
			}
			var navigationType = typeof(TNavigationType);
			var navigationProperty = typeof(TOther).FastGetProperties()
				.FirstOrDefault(p => navigationType.IsAssignableFrom(p.PropertyType));
			return navigationProperty?.Name;
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			// Unsupported options: Length, Unique, Index,
			// CustomSqlType, WithSerialization
			options = options ?? new EntityMappingOptions();
			var referenceBuilder = Builder
				.HasOne(memberExpression)
				.WithMany(GetNavigationPropertyName<TOther, IEnumerable<T>>(options));
			if (!string.IsNullOrEmpty(options.Column)) {
				referenceBuilder = referenceBuilder.HasConstraintName(options.Column);
			}
			if (options.Nullable == true) {
				referenceBuilder = referenceBuilder.IsRequired(false);
			} else if (options.Nullable == false) {
				referenceBuilder = referenceBuilder.IsRequired(true);
			}
			// Cascade should specified on parent side, but just support this option
			if (options.CascadeDelete == false) {
				referenceBuilder.OnDelete(DeleteBehavior.Restrict);
			} else if (options.CascadeDelete == true) {
				referenceBuilder.OnDelete(DeleteBehavior.Cascade);
			}
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			// Unsupported options: Column, Length, Unique,
			// Nullable, Index, CustomSqlType, WithSerialization
			options = options ?? new EntityMappingOptions();
			var collectionBuilder = Builder
				.HasMany(memberExpression)
				.WithOne(GetNavigationPropertyName<TChild, T>(options));
			if (options.CascadeDelete == false) {
				collectionBuilder = collectionBuilder.OnDelete(DeleteBehavior.Restrict);
			} else {
				collectionBuilder = collectionBuilder.OnDelete(DeleteBehavior.Cascade); // true or default
			}
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError(
				"Entity framework core not support many-to-many yet, " +
				"see https://github.com/aspnet/EntityFramework/issues/1368 " +
				$"expression: {memberExpression}");
		}
	}
}
