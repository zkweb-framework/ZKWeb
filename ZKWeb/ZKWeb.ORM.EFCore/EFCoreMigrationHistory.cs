using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Custom migration history entity<br/>
	/// <br/>
	/// </summary>
	internal class EFCoreMigrationHistory {
		/// <summary>
		/// Migration revision<br/>
		/// <br/>
		/// </summary>
		public virtual int Revision { get; set; }
		/// <summary>
		/// Model snapshot<br/>
		/// <br/>
		/// </summary>
		public virtual string Model { get; set; }
		/// <summary>
		/// Entity Framework Core product version<br/>
		/// <br/>
		/// </summary>
		public virtual string ProductVersion { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public EFCoreMigrationHistory() { }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="model">Model snapshot</param>
		public EFCoreMigrationHistory(string model) {
			Model = model;
			ProductVersion = ProductInfo.GetVersion();
		}

		/// <summary>
		/// Configure entity model<br/>
		/// <br/>
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
