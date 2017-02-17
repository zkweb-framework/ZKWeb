using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Custom migration history entity
	/// </summary>
	internal class EFCoreMigrationHistory {
		/// <summary>
		/// Migration revision
		/// </summary>
		public virtual int Revision { get; set; }
		/// <summary>
		/// Model snapshot
		/// </summary>
		public virtual string Model { get; set; }
		/// <summary>
		/// Entity Framework Core product version
		/// </summary>
		public virtual string ProductVersion { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public EFCoreMigrationHistory() { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="model">Model snapshot</param>
		public EFCoreMigrationHistory(string model) {
			Model = model;
			ProductVersion = ProductInfo.GetVersion();
		}

		/// <summary>
		/// Configure entity model
		/// </summary>
		/// <param name="builder">Model builder</param>
		public virtual void Configure(ModelBuilder builder) {
			var typeBuilder = builder.Entity<EFCoreMigrationHistory>()
				.ToTable("__ZKWeb_EFMigrationHistory");
			typeBuilder.HasKey(h => h.Revision);
			typeBuilder.Property(h => h.Model).IsRequired();
			typeBuilder.Property(h => h.ProductVersion).IsRequired();
		}
	}
}
