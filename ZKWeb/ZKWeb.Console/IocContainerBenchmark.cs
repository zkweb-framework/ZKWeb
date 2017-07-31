namespace ZKWeb.Console {
	using System;
	using ZKWebStandard.Ioc;

	/// <summary>
	/// Benchmark for builtin IoC Container<br/>
	/// 内置IoC容器的性能测试<br/>
	/// </summary>
	public static class IocContainerBenchmark {
		public interface BenchmarkInterface { }
		public class BenchmarkClass : BenchmarkInterface { }

		private static void BenchmarkRegisterTransient() {
			Console.WriteLine("BenchmarkRegisterTransient");
			var begin = DateTime.UtcNow;
			IContainer container = new Container();
			for (var i = 0; i < 1000000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Transient, i);
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		private static void BenchmarkRegisterSingleton() {
			Console.WriteLine("BenchmarkRegisterSingleton");
			var begin = DateTime.UtcNow;
			IContainer container = new Container();
			for (var i = 0; i < 1000000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Singleton, i);
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		private static void BenchmarkResolveTransient() {
			Console.WriteLine("BenchmarkResolveTransient");
			IContainer container = new Container();
			container.RegisterMany<BenchmarkClass>();
			for (var i = 0; i < 10000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Transient, i);
			}
			var begin = DateTime.UtcNow;
			for (var i = 0; i < 100000000; ++i) {
				container.Resolve<BenchmarkInterface>();
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		private static void BenchmarkResolveSingleton() {
			Console.WriteLine("BenchmarkResolveSingleton");
			IContainer container = new Container();
			container.RegisterMany<BenchmarkClass>();
			for (var i = 0; i < 10000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Singleton, i);
			}
			var begin = DateTime.UtcNow;
			for (var i = 0; i < 100000000; ++i) {
				container.Resolve<BenchmarkInterface>();
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		private static void BenchmarkResolveManyTransient() {
			Console.WriteLine("BenchmarkResolveManyTransient");
			IContainer container = new Container();
			container.RegisterMany<BenchmarkClass>();
			for (var i = 0; i < 10000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Transient, i);
			}
			var begin = DateTime.UtcNow;
			for (var i = 0; i < 10000000; ++i) {
				foreach (var instance in container.ResolveMany<BenchmarkInterface>()) { }
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		private static void BenchmarkResolveManySingleton() {
			Console.WriteLine("BenchmarkResolveManySingleton");
			IContainer container = new Container();
			container.RegisterMany<BenchmarkClass>();
			for (var i = 0; i < 10000; ++i) {
				container.RegisterMany<BenchmarkClass>(ReuseType.Singleton, i);
			}
			var begin = DateTime.UtcNow;
			for (var i = 0; i < 10000000; ++i) {
				foreach (var instance in container.ResolveMany<BenchmarkInterface>()) { }
			}
			Console.WriteLine("Used {0}s", (DateTime.UtcNow - begin).TotalSeconds);
		}

		/// <summary>
		/// Start benchmark<br/>
		/// 开始性能测试<br/>
		/// </summary>
		public static void Start() {
			BenchmarkRegisterTransient();
			BenchmarkRegisterSingleton();
			BenchmarkResolveTransient();
			BenchmarkResolveSingleton();
			BenchmarkResolveManyTransient();
			BenchmarkResolveManySingleton();
		}
	}
}
