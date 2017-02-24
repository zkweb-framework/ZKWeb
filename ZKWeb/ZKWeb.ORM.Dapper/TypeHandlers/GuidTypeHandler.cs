using Dapper;
using System;
using System.Data;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Handle guid type
	/// It's needed because mysql use varchar for guid
	/// </summary>
	internal class GuidTypeHandler : SqlMapper.TypeHandler<Guid> {
		/// <summary>
		/// Convert value to guid field
		/// </summary>
		public override Guid Parse(object value) {
			return value.ConvertOrDefault<Guid>();
		}

		/// <summary>
		/// Convert guid field to value
		/// </summary>
		public override void SetValue(IDbDataParameter parameter, Guid value) {
			if (parameter.DbType == DbType.Guid) {
				parameter.Value = value;
			} else {
				parameter.Value = value.ToString();
			}
		}
	}
}
