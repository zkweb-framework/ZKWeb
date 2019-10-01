using System;

namespace ZKWebStandard.Ioc
{
    /// <summary>
    /// The delegate type of factory function<br/>
    /// 工厂函数的委托类型<br/>
    /// </summary>
    public delegate object ContainerFactoryDelegate(IContainer container, Type serviceType);
}
