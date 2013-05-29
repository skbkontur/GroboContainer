using System;
using System.Collections.Generic;
using System.Text;

namespace GroboContainer.Impl.Logging
{
    public class Log : ILog
    {
        private static readonly int[] depthChange;
        private readonly List<LogItem> items = new List<LogItem>();
        private int crashIndex = int.MaxValue;

        static Log()
        {
            depthChange = new int[(int) ItemType.Reused + 1];
            depthChange[(int) ItemType.Constructing] = 1;
            depthChange[(int) ItemType.Get] = 1;
            depthChange[(int) ItemType.Create] = 1;
            depthChange[(int) ItemType.GetAll] = 1;

            depthChange[(int) ItemType.EndGet] = -1;
            depthChange[(int) ItemType.EndCreate] = -1;
            depthChange[(int) ItemType.EndGetAll] = -1;
            depthChange[(int) ItemType.Constructed] = -1;

            depthChange[(int) ItemType.Reused] = 0;
        }

        #region ILog Members

        public void BeginConstruct(Type implementationType)
        {
            items.Add(new LogItem(ItemType.Constructing, implementationType));
        }

        public void EndConstruct(Type implementationType)
        {
            items.Add(new LogItem(ItemType.Constructed, implementationType));
        }

        public void Reused(Type implementationType)
        {
            items.Add(new LogItem(ItemType.Reused, implementationType));
        }

        public void Crash()
        {
            if (crashIndex == int.MaxValue)
                crashIndex = items.Count;
        }

        public void BeginGet(Type type)
        {
            items.Add(new LogItem(ItemType.Get, type));
        }

        public void EndGet(Type type)
        {
            items.Add(new LogItem(ItemType.EndGet, type));
        }

        public void BeginCreate(Type type)
        {
            items.Add(new LogItem(ItemType.Create, type));
        }

        public void EndCreate(Type type)
        {
            items.Add(new LogItem(ItemType.EndCreate, type));
        }

        public void BeginGetAll(Type type)
        {
            items.Add(new LogItem(ItemType.GetAll, type));
        }

        public void EndGetAll(Type type)
        {
            items.Add(new LogItem(ItemType.EndGetAll, type));
        }

        public string GetLog()
        {
            int depth = 0;
            var builder = new StringBuilder();
            int index = 0;
            foreach (LogItem item in items)
            {
                if (crashIndex <= index) break;
                int delta = depthChange[(int) item.ItemType];
                if (delta < 0)
                    depth += delta;
                builder.Append(new string(' ', depth));
                item.AppendTo(builder);
                if (delta > 0)
                    depth += delta;
                index++;
            }
            return builder.ToString();
        }

        #endregion
    }
}