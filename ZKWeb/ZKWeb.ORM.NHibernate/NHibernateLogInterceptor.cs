using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Database;
using ZKWebStandard.Ioc;

namespace ZKWeb.ORM.NHibernate
{
    /// <summary>
    /// The interceptor used to log sql statements<br/>
    /// 用于记录sql语句的拦截器<br/>
    /// </summary>
    public class NHibernateLogInterceptor : EmptyInterceptor
    {
        /// <summary>
        /// Database command logger<br/>
        /// 数据库命令记录器<br/>
        /// </summary>
        public IDatabaseCommandLogger CommandLogger { get; set; }
        /// <summary>
        /// Associated database context<br/>
        /// 关联的数据库上下文<br/>
        /// </summary>
        public NHibernateDatabaseContext Context { get; set; }

        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public NHibernateLogInterceptor()
        {
            CommandLogger = Application.Ioc.Resolve<IDatabaseCommandLogger>(IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Log sql statement<br/>
        /// 记录sql语句<br/>
        /// </summary>
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            if (CommandLogger != null && Context != null)
            {
                CommandLogger.LogCommand(Context, sql.ToString(), null);
            }
            return base.OnPrepareStatement(sql);
        }
    }
}
