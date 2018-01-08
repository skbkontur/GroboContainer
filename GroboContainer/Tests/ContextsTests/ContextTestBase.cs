using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using Rhino.Mocks;
using Tests.NMockHelpers;

namespace Tests.ContextsTests
{
    public abstract class ContextTestBase : CoreTestBase
    {
        protected static void CheckGet(IContextHolder holder)
        {
            var container = GetMock<IInternalContainer>();
            container.Expect(c => c.CreateNewLog()).Return(null);
            var context = holder.GetContext(container);
            Assert.That(context, Is.InstanceOf<InjectionContext>());
            container.Expect(c => c.CreateNewLog()).Return(null);
            ((InjectionContext) context).AssertEqualsTo(new InjectionContext(container));
        }
    }
}