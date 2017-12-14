using System.Data;
using System.Diagnostics;

namespace ZKWeb.Database.Wrappers {
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
	/// <summary>
	/// Wrapper for IDbCommand, use to log commands<br/>
	/// IDbCommand的包装类, 用于记录命令<br/>
	/// </summary>
	public class DbCommandWrapper : IDbCommand {
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
#pragma warning disable CS1591
		private readonly IDbCommand Command;
		private readonly IDatabaseContext Context;

		public DbCommandWrapper(IDbCommand command, IDatabaseContext context) {
			Command = command;
			Context = context;
		}

		public IDbConnection Connection {
			get => Command.Connection;
			set => Command.Connection = value;
		}

		public IDbTransaction Transaction {
			get => Command.Transaction;
			set => Command.Transaction = value;
		}

		public string CommandText {
			get => Command.CommandText;
			set => Command.CommandText = value;
		}

		public int CommandTimeout {
			get => Command.CommandTimeout;
			set => Command.CommandTimeout = value;
		}

		public CommandType CommandType {
			get => Command.CommandType;
			set => Command.CommandType = value;
		}

		public IDataParameterCollection Parameters => Command.Parameters;

		public UpdateRowSource UpdatedRowSource {
			get => Command.UpdatedRowSource;
			set => Command.UpdatedRowSource = value;
		}

		public void Cancel() {
			Command.Cancel();
		}

		public IDbDataParameter CreateParameter() {
			return Command.CreateParameter();
		}

		public void Dispose() {
			Command.Dispose();
		}

		public int ExecuteNonQuery() {
			var logger = Context.CommandLogger;
			if (logger == null) {
				return Command.ExecuteNonQuery();
			} else {
				var watch = Stopwatch.StartNew();
				var result = Command.ExecuteNonQuery();
				var elapsedMilliseconds = watch.ElapsedMilliseconds;
				logger.LogCommand(Context, CommandText, new { elapsedMilliseconds, parameters = Parameters });
				return result;
			}
		}

		public IDataReader ExecuteReader() {
			var logger = Context.CommandLogger;
			if (logger == null) {
				return Command.ExecuteReader();
			} else {
				logger.LogCommand(Context, CommandText, new { parameters = Parameters });
				return Command.ExecuteReader();
			}
		}

		public IDataReader ExecuteReader(CommandBehavior behavior) {
			var logger = Context.CommandLogger;
			if (logger == null) {
				return Command.ExecuteReader(behavior);
			} else {
				logger.LogCommand(Context, CommandText, new { parameters = Parameters });
				return Command.ExecuteReader(behavior);
			}
		}

		public object ExecuteScalar() {
			var logger = Context.CommandLogger;
			if (logger == null) {
				return Command.ExecuteScalar();
			} else {
				var watch = Stopwatch.StartNew();
				var result = Command.ExecuteScalar();
				var elapsedMilliseconds = watch.ElapsedMilliseconds;
				logger.LogCommand(Context, CommandText, new { elapsedMilliseconds, parameters = Parameters });
				return result;
			}
		}

		public void Prepare() {
			Command.Prepare();
		}
#pragma warning restore CS1591
	}
}
