using System;
using System.Linq;
using System.Text;

namespace Tests.PerfTests
{
    public class ExponentialHistogram
    {
        private readonly object locker = new object();
        private readonly long[] limits;
        private readonly long[] buckets;
        private readonly int scale;
        private long totalCount;
        private double totalValue;
        private long min;
        private long max;

        public ExponentialHistogram(int scale)
        {
            this.scale = scale;
            limits = new long[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4094, 8192, 16384, 32768, 65536, 131072 }.Select(x => x * scale).ToArray();
            buckets = new long[limits.Length];
            totalCount = 0;
            totalValue = 0.0;
            min = long.MaxValue;
            max = -1;
        }

        public long TotalCount
        {
            get
            {
                lock (locker)
                    return totalCount;
            }
        }

        public double TotalValue
        {
            get
            {
                lock (locker)
                    return totalValue / scale;
            }
        }

        public long Min
        {
            get
            {
                lock (locker)
                    return min / scale;
            }
        }

        public long Max
        {
            get
            {
                lock (locker)
                    return max / scale;
            }
        }

        public void Register(long value)
        {
            lock (locker)
                DoRegister(value);
        }

        private void DoRegister(long value)
        {
            if (value < 0)
                throw new InvalidOperationException(string.Format("Invalid value: {0}", value));
            int i;
            for (i = 1; i < limits.Length && value >= limits[i]; i++)
            {
            }
            buckets[i - 1]++;
            totalCount++;
            totalValue += value;
            if (value < min)
                min = value;
            if (value > max)
                max = value;
        }

        public double GetAverage()
        {
            lock (locker)
                return DoGetAverage() / scale;
        }

        private double DoGetAverage()
        {
            if (totalCount == 0)
                return -1;
            return Math.Round(totalValue / totalCount);
        }

        public long GetPercentile(double percent)
        {
            lock (locker)
                return DoGetPercentile(percent) / scale;
        }

        private long DoGetPercentile(double percent)
        {
            if (percent <= 0 || percent > 1.0)
                throw new InvalidOperationException(string.Format("Percent must be in range (0.00, 1.00] but was: {0}", percent));
            var itemsCount = 0L;
            var itemsThreshold = (long)Math.Ceiling(percent * totalCount);
            for (var i = 0; i < limits.Length; i++)
            {
                if (itemsCount >= itemsThreshold)
                    return limits[i];
                itemsCount += buckets[i];
            }
            return max;
        }

        public override string ToString()
        {
            var average = GetAverage();
            var percentile95Latency = GetPercentile(0.95);
            var percentile99Latency = GetPercentile(0.99);
            var percentile999Latency = GetPercentile(0.999);
            var sb = new StringBuilder();
            for (var i = 0; i < limits.Length; i++)
                sb.AppendFormat("{0}:{1}, ", limits[i] / scale, buckets[i]);
            var histogram = sb.ToString(0, sb.Length - 2);
            return string.Format("TotalValue: {0}; TotalCount: {1}; Avg: {2}; 95%%: {3}; 99%%: {4}; 99.9%%: {5}; Min: {6}; Max: {7}; Histogram: {{{8}}}",
                TotalValue.ToString("F0"), TotalCount, average.ToString("F0"), percentile95Latency, percentile99Latency, percentile999Latency, Min, Max, histogram);
        }
    }
}