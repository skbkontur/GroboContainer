using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using TestCore;

namespace Tests.ContextsTests
{
    public abstract class ContextTestBase : CoreTestBase
    {
        protected static void CheckGet(IContextHolder holder)
        {
            var container = NewMock<IInternalContainer>();
            IInjectionContext context = holder.GetContext(container);
            Assert.IsInstanceOfType(typeof (InjectionContext), context);
            ((InjectionContext) context).AssertEqualsTo(new InjectionContext(container));
        }
    }
}