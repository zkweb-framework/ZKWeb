using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Custom migration history entity<br/>
	/// 自定义的迁移历史实体<br/>
	/// </summary>
	public class EFCoreMigrationHistory {
		/// <summary>
		/// Migration revision<br/>
		/// 迁移版本<br/>
		/// </summary>
		public virtual int Revision { get; set; }
		/// <summary>
		/// Model snapshot<br/>
		/// 模型快照<br/>
		/// </summary>
		public virtual string Model { get; set; }
		/// <summary>
		/// Entity Framework Core product version<br/>
		/// Entity Framework Core的产品版本<br/>
		/// </summary>
		public virtual string ProductVersion { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public EFCoreMigrationHistory() { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="model">Model snapshot</param>
		public EFCoreMigrationHistory(string model) {
			Model = model;
			ProductVersion = ProductInfo.GetVersion();
		}

		/// <summary>
		/// Configure entity model<br/>
		/// 配置实体模型<br/>
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
