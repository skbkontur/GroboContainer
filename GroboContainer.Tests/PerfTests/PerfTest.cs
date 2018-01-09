using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using GroboContainer.Core;
using GroboContainer.Impl;
using NUnit.Framework;

namespace Tests.PerfTests
{
    public class PerfTest : TestBase
    {
        public interface IIntf1
        {
        }

        public class Impl1 : IIntf1
        {
        }

        [Test]
        [Category("LongRunning")]
        public void Test()
        {
            const int containerCount = 1000;
            const int threadCount = 4;
            var containers = new IContainer[containerCount];
            var threads = new Thread[threadCount];
            for (var i = 0; i < containers.Length; i++)
                containers[i] = new Container(new ContainerConfiguration(Assembly.GetExecutingAssembly()));
            int index = 0;
            var start = new ManualResetEvent(false);
            var times = new ConcurrentBag<long>();
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var stopwatch = new Stopwatch();
                    start.WaitOne();
                    while (true)
                    {
                        var containerIndex = Interlocked.CompareExchange(ref index, 0, 0);
                        if (containerIndex >= containers.Length)
                            break;
                        stopwatch.Restart();
                        containers[containerIndex].Get<IIntf1>();
                        stopwatch.Stop();
                        times.Add(stopwatch.ElapsedTicks);
                    }
                });
                threads[i].Start();
            }
            start.Set();
            for (int i = 0; i < containers.Length; i++)
            {
                Thread.Sleep(1);
                Interlocked.Increment(ref index);
            }
            foreach (var thread in threads)
                thread.Join();
            var histogram = new ExponentialHistogram(1);
            foreach (var time in times)
                histogram.Register(time);
            Console.Out.WriteLine(histogram.ToString());
        }
    }
}