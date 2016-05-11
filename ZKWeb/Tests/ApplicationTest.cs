using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests {
	[UnitTest]
	class ApplicationTest {
		public abstract class Base { }
		public class A : Base { }
		public class B : Base { }

		public void OverrideIoc() {
			Assert.IsTrue(!Application.Ioc.IsRegistered<Base>());
			using (Application.OverrideIoc()) {
				Application.Ioc.Register<Base, A>();
				Assert.IsTrue(Application.Ioc.Resolve<Base>() is A);
				using (Application.OverrideIoc()) {
					Application.Ioc.Register<Base, B>();
					Assert.IsTrue(Application.Ioc.Resolve<Base>() is B);
				}
				Assert.IsTrue(Application.Ioc.Resolve<Base>() is A);
			}
			Assert.IsTrue(!Application.Ioc.IsRegistered<Base>());
		}
	}
}
