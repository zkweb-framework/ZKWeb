using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using System.Linq.Expressions;
using NHibernate.Linq;
using DryIoc;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using System.Data;

namespace ZKWeb.Core {
	/// <summary>
	/// 数据库上下文
	/// 这个类用于包装对数据库的操作和触发以下事件
	///		IDataAddCallback
	///		IDataUpdateCallback
	///		IDataDeleteCallback
	/// NHibernate本身支持事件但不适合使用（修改数据需要同时改状态，需要绑定的事件较多等）
	/// 例子（查询，[]要替换）
	///		using (var context = databaseManager.GetContext()) {
	///			var data = context.Get[T](d => d.Id == id);
	///			var list = context.Query[t]().Where(d => d.Name.Contains("test")).Take(5).ToList();
	///		}
	/// 例子（添加）
	///		using (var context = databaseManager.GetContext()) {
	///			context.Save(data);
	///			context.SaveChanges();
	///		}
	///	例子（修改，[]要替换）
	///		using (var context = databaseManager.GetContext()) {
	///			var data = context.Get[T](d => d.Id == id);
	///			if (data != null) {
	///				context.Save(data, d => d.Remark = "test update");
	///				context.SaveChanges();
	///			}
	///		}
	/// 例子（删除）
	///		using (var context = databaseManager.GetContext()) {
	///			context.DeleteWhere(d => d.Expired);
	///			context.SaveChanges();
	///		}
	/// </summary>
	public class DatabaseContext : IDisposable {
		/// <summary>
		/// 数据库会话
		/// </summary>
		public ISession Session { get; private set; }
		/// <summary>
		/// 数据库事务
		/// </summary>
		public ITransaction Transaction { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="sessionFactory">数据库会话生成器</param>
		/// <param name="isolationLevel">事务的隔离等级</param>
		public DatabaseContext(ISessionFactory sessionFactory, IsolationLevel isolationLevel) {
			Session = sessionFactory.OpenSession();
			Transaction = Session.BeginTransaction(isolationLevel);
		}

		/// <summary>
		/// 释放创建的数据库会话和事务
		/// </summary>
		public void Dispose() {
			Transaction.Dispose();
			Session.Dispose();
		}

		/// <summary>
		/// 从数据库中获取满足条件的单个对象
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="expression">表达式</param>
		/// <returns></returns>
		public T Get<T>(Expression<Func<T, bool>> expression)
			where T : class {
			return Session.Query<T>().FirstOrDefault(expression);
		}

		/// <summary>
		/// 获取满足条件的对象数量
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="expression">表达式</param>
		/// <returns></returns>
		public long Count<T>(Expression<Func<T, bool>> expression)
			where T : class {
			return Session.Query<T>().LongCount(expression);
		}

		/// <summary>
		/// 获取指定类型的查询器
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <returns></returns>
		public IQueryable<T> Query<T>()
			where T : class {
			return Session.Query<T>();
		}

		/// <summary>
		/// 保存数据到数据库
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="data">数据</param>
		/// <param name="update">更新函数，这里的修改可以在回调的BeforeSave和AfterSave之间对比</param>
		/// <returns></returns>
		public void Save<T>(ref T data, Action<T> update = null)
			where T : class {
			var callbacks = Application.Ioc.ResolveMany<IDataSaveCallback<T>>();
			var dataLocal = data; // lambda中不能使用ref参数
			callbacks.ForEach(c => c.BeforeSave(this, dataLocal));
			if (update != null) {
				update(dataLocal);
			}
			dataLocal = Session.Merge(dataLocal); // 如果数据不在会话中会重新创建并返回
			Session.Flush();
			callbacks.ForEach(c => c.AfterSave(this, dataLocal));
			data = dataLocal;
		}

		/// <summary>
		/// 从数据库删除数据
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="data">删除的数据</param>
		public void Delete<T>(T data)
			where T : class {
			var callbacks = Application.Ioc.ResolveMany<IDataDeleteCallback<T>>();
			callbacks.ForEach(c => c.BeforeDelete(this, data));
			Session.Delete(data);
			Session.Flush();
			callbacks.ForEach(c => c.AfterDelete(this, data));
		}

		/// <summary>
		/// 批量更新
		/// 返回更新的数量
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="expression">更新条件</param>
		/// <param name="update">更新函数</param>
		public long UpdateWhere<T>(Expression<Func<T, bool>> expression, Action<T> update)
			where T : class {
			long count = 0;
			Query<T>().Where(expression).ForEach(d => { Save(ref d, update); ++count; });
			return count;
		}

		/// <summary>
		/// 批量删除
		/// 返回删除的数量
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="expression">删除条件</param>
		public long DeleteWhere<T>(Expression<Func<T, bool>> expression)
			where T : class {
			long count = 0;
			Query<T>().Where(expression).ForEach(d => { Delete(d); ++count; });
			return count;
		}

		/// <summary>
		/// 提交所有修改
		/// 如数据库上下文在删除前没有调用此函数则所有修改都不会被提交
		/// </summary>
		public void SaveChanges() {
			Transaction.Commit();
		}
	}
}
