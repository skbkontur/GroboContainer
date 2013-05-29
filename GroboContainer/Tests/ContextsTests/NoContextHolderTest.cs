using GroboContainer.Impl.Contexts;
using NUnit.Framework;

namespace Tests.ContextsTests
{
    public class NoContextHolderTest : ContextTestBase
    {
        [Test]
        public void TestGetKill()
        {
            IContextHolder holder = new NoContextHolder();
            CheckGet(holder);
            holder.KillContext();
            CheckGet(holder);
        }
    }
}