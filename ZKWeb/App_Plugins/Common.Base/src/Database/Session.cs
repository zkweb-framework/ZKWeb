using DryIocAttributes;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Functions;

namespace Common.Base.Database {
	/// <summary>
	/// 会话
	/// </summary>
	public class Session {
		/// <summary>
		/// 会话Id
		/// </summary>
		public virtual string Id { get; set; }
		/// <summary>
		/// 关联Id
		/// 一般是用户Id
		/// </summary>
		public virtual long ReleatedId { get; set; }
		/// <summary>
		/// 会话数据的内容
		/// </summary>
		public virtual string ItemsJson { get; set; }
		/// <summary>
		/// 会话对应的Ip地址
		/// </summary>
		public virtual string IpAddress { get; set; }
		/// <summary>
		/// 是否记住登录
		/// </summary>
		public virtual bool RememberLogin { get; set; }
		/// <summary>
		/// 过期时间
		/// </summary>
		public virtual DateTime Expires { get; set; }

		/// <summary>
		/// 过期时间是否已更新
		/// 不保存到数据库中
		/// </summary>
		public virtual bool ExpiresUpdated { get; set; }
		/// <summary>
		/// 会话数据
		/// 只通过ItemsJson保存到数据库中
		/// </summary>
		public virtual Dictionary<string, object> Items { get; set; }

		/// <summary>
		/// 重新生成Id
		/// </summary>
		public virtual void ReGenerateId() {
			this.Id = HttpServerUtility.UrlTokenEncode(RandomUtils.SystemRandomBytes(20));
		}

		/// <summary>
		/// 设置会话最少在指定的时间后过期
		/// 当前会话的过期时间比指定的时间要晚时不更新当前的过期时间
		/// </summary>
		/// <param name="span">最少在这个时间后过期</param>
		public virtual void SetExpiresAtLeast(TimeSpan span) {
			var expires = DateTime.UtcNow + span;
			if (this.Expires < expires) {
				this.Expires = expires;
				this.ExpiresUpdated = true;
			}
		}
	}

	/// <summary>
	/// 会话的数据库结构
	/// </summary>
	[ExportMany]
	public class SessionMap : ClassMap<Session> {
		/// <summary>
		/// 初始化
		/// </summary>
		public SessionMap() {
			Id(s => s.Id);
			Map(s => s.ReleatedId);
			Map(s => s.ItemsJson).Length(0xffff);
			Map(s => s.IpAddress);
			Map(s => s.RememberLogin);
			Map(s => s.Expires).Index("Idx_Expires");
		}
	}
}
