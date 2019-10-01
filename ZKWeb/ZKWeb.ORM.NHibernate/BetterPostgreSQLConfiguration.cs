using FluentNHibernate.Cfg.Db;

namespace ZKWeb.ORM.NHibernate
{
#pragma warning disable S101 // Types should be named in camel case
    /// <summary>
    /// Better postgre sql configuration<br/>
    /// 更好的PostgreSQL配置类<br/>
    /// </summary>
    public class BetterPostgreSQLConfiguration : PostgreSQLConfiguration
    {
#pragma warning restore S101 // Types should be named in camel case
        /// <summary>
        /// Better configuration<br/>
        /// 获取更好的配置<br/>
        /// </summary>
        public static PostgreSQLConfiguration Better
        {
            get { return new BetterPostgreSQLConfiguration().Dialect<BetterPostgreSQLDialect>(); }
        }
    }
}
