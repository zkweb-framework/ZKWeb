using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZKWeb.Database;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core entity mapping builder
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	internal class EFCoreEntityMappingBuilder<T> : IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// Entity Framework Core type builder
		/// </summary>
		private EntityTypeBuilder<T> Builder { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="builder">Model builder</param>
		public EFCoreEntityMappingBuilder(
			ModelBuilder builder) {
			Builder = builder.Entity<T>();
			// Set table name with registered handlers
			var tableName = typeof(T).Name;
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			handlers.ForEach(h => h.ConvertTableName(ref tableName));
			Builder = Builder.ToTable(tableName);
			// Configure with registered providers
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
		}

		/// <summary>
		/// Specify the primary key for this entity
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
		/// Create a member mapping
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
				Builder.HasAlternateKey(Expression.Lambda<Func<T, object>>(
					Expression.Convert(memberExpression.Body, typeof(object)),
					memberExpression.Parameters));
			}
			if (options.Nullable == true) {
				propertyBuilder = propertyBuilder.IsRequired(true);
			} else if (options.Nullable == false) {
				propertyBuilder = propertyBuilder.IsRequired(false);
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
				throw new NotSupportedException(
					"Entity framework core not support custom type mapping yet, " +
					"see https://github.com/aspnet/EntityFramework/issues/242");
			}
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			// Unsupported options: Length, Unique, Index,
			// CustomSqlType, CascadeDelete, WithSerialization
			options = options ?? new EntityMappingOptions();
			var referenceBuilder = Builder.HasOne(memberExpression).WithMany();
			if (!string.IsNullOrEmpty(options.Column)) {
				referenceBuilder = referenceBuilder.HasConstraintName(options.Column);
			}
			if (options.Nullable == true) {
				referenceBuilder = referenceBuilder.IsRequired(true);
			} else if (options.Nullable == false) {
				referenceBuilder = referenceBuilder.IsRequired(false);
			}
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			// Unsupported options: Column, Length, Unique,
			// Nullable, Index, CustomSqlType, WithSerialization
			options = options ?? new EntityMappingOptions();
			var collectionBuilder = Builder.HasMany(memberExpression).WithOne();
			if (options.CascadeDelete == true) {
				collectionBuilder = collectionBuilder.OnDelete(DeleteBehavior.Cascade);
			} else if (options.CascadeDelete == false) {
				collectionBuilder = collectionBuilder.OnDelete(DeleteBehavior.Restrict);
			}
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			throw new NotSupportedException(
				"Entity framework core not support many-to-many yet, " +
				"see https://github.com/aspnet/EntityFramework/issues/1368");
		}
	}
}
