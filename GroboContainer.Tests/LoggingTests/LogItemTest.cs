using System.Text;
using GroboContainer.Impl.Logging;
using NUnit.Framework;

namespace Tests.LoggingTests
{
    public class LogItemTest : CoreTestBase
    {
        private static string ToStr(LogItem item)
        {
            var builder = new StringBuilder();
            item.AppendTo(builder);
            return builder.ToString();
        }

        [Test]
        public void TestStupid()
        {
            var item = new LogItem(ItemType.Get, typeof (int));
            Assert.AreEqual("Get<System.Int32>()\r\n", ToStr(item));
        }
    }
}