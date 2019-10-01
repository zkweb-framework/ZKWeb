using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests
{
    [Tests]
    class ApplicationTest
    {
        public abstract class Base { }
        public class A : Base { }
        public class B : Base { }

        public void OverrideIoc()
        {
            Assert.Equals(Application.Ioc.Resolve<Base>(IfUnresolved.ReturnDefault), null);
            using (Application.OverrideIoc())
            {
                Application.Ioc.Register<Base, A>();
                Assert.IsTrue(Application.Ioc.Resolve<Base>() is A);
                using (Application.OverrideIoc())
                {
                    Application.Ioc.Unregister<Base>();
                    Application.Ioc.Register<Base, B>();
                    Assert.IsTrue(Application.Ioc.Resolve<Base>() is B);
                }
                Assert.IsTrue(Application.Ioc.Resolve<Base>() is A);
            }
            Assert.Equals(Application.Ioc.Resolve<Base>(IfUnresolved.ReturnDefault), null);
        }
    }
}
