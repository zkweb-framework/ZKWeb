using Dapper;
using System;
using System.Data;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.Dapper.TypeHandlers {
	/// <summary>
	/// Handle guid type<br/>
	/// It's needed because mysql use varchar for guid<br/>
	/// 处理guid类型<br/>
	/// <br/>
	/// </summary>
	public class GuidTypeHandler : SqlMapper.TypeHandler<Guid> {
		/// <summary>
		/// Convert value to guid field<br/>
		/// 转换值到guid字段<br/>
		/// </summary>
		public override Guid Parse(object value) {
			return value.ConvertOrDefault<Guid>();
		}

		/// <summary>
		/// Convert guid field to value<br/>
		/// 转换值到guid字段<br/>
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
