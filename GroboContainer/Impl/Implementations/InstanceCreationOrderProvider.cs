using System.Threading;

namespace GroboContainer.Impl.Implementations
{
    static class InstanceCreationOrderProvider
    {
        public static int Next => Interlocked.Increment(ref current);

        private static int current = 0;
    }
}