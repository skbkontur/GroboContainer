using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using TestCore;
using Rhino.Mocks;

namespace Tests.ContextsTests
{
    public abstract class ContextTestBase : CoreTestBase
    {
        protected static void CheckGet(IContextHolder holder)
        {
            var container =GetMock<IInternalContainer>();
            container.Expect(c => c.CreateNewLog()).Return(null);
            IInjectionContext context = holder.GetContext(container);
            Assert.IsInstanceOfType(typeof (InjectionContext), context);
            container.Expect(c => c.CreateNewLog()).Return(null);
            ((InjectionContext) context).AssertEqualsTo(new InjectionContext(container));
        }
    }
}