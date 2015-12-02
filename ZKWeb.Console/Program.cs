using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZKWeb.Console {
	using System;

	class Program {
		static void Main(string[] args) {
			var a = new ReaderWriterLockSlim();
			a.EnterReadLock();
			a.EnterReadLock();
			Console.WriteLine("done");
		}
	}
}
