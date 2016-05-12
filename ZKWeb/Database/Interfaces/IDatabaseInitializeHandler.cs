using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Database.Interfaces {
	/// <summary>
	/// 数据库初始化处理器
	/// </summary>
	public interface IDatabaseInitializeHandler {
		/// <summary>
		/// 数据库初始化时的处理
		/// </summary>
		/// <param name="configuration"></param>
		void OnInitialize(FluentConfiguration configuration);
	}
}
