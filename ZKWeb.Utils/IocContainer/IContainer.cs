using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// Ioc容器的接口
	/// </summary>
	public interface IContainer : IRegistrator, IResolver, ICloneable, IDisposable { }
}
