using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZKWebStandard.Ioc {
	internal static class ContainerFactoriesCache<TService> {
		internal static IList<Func<object>> Factories = null;
	}
}
