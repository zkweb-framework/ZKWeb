using NHibernate.Dialect;
using System.Data;

namespace ZKWeb.ORM.NHibernate
{
#pragma warning disable S101 // Types should be named in camel case
    /// <summary>
    /// Better pgsql dialect<br/>
    /// What's improved<br/>
    /// - Support Guid type<br/>
    /// 更好的pgsql配置<br/>
    /// 改进点<br/>
    /// - 支持Guid类型<br/>
    /// </summary>
    public class BetterPostgreSQLDialect : PostgreSQLDialect
    {
#pragma warning restore S101 // Types should be named in camel case
        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public BetterPostgreSQLDialect()
        {
            RegisterColumnType(DbType.Guid, "uuid");
        }
    }
}
