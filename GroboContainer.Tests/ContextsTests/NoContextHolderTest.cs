using GroboContainer.Impl.Contexts;

using NUnit.Framework;

namespace GroboContainer.Tests.ContextsTests
{
    public class NoContextHolderTest : ContextTestBase
    {
        [Test]
        public void TestGetKill()
        {
            IContextHolder holder = NoContextHolder.Instance;
            CheckGet(holder);
            holder.KillContext();
            CheckGet(holder);
        }
    }
}