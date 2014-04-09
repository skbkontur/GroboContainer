using System;
using System.Collections.Generic;
using System.Text;

namespace GroboContainer.Impl.Logging
{
    public class ShortLog : ILog
    {
        private readonly string containerName;
        private static readonly int[] depthChange;
        private readonly List<LogItem> items = new List<LogItem>();
        private bool wasCrash;

        static ShortLog()
        {
            depthChange = LogSettings.depthChange;
        }

        public ShortLog(string containerName)
        {
            this.containerName = containerName;
        }

        private void AddItem(LogItem item)
        {
            if(wasCrash)
                return;
            int add = depthChange[(int) item.ItemType];
            switch (add)
            {
                case +1:
                    items.Add(item);
                    break;
                case -1:
                    items.RemoveAt(items.Count - 1);
                    break;
                case 0:
                    break;
                //    goto case 0;
                //case 0:
                //    items[items.Count - 1] = item; //todo ??
                //    break;
                default:
                    throw new NotSupportedException(string.Format("bad item {0}", item.ItemType));
            }
        }

        #region ILog Members

        public void BeginConstruct(Type implementationType)
        {
            AddItem(new LogItem(ItemType.Constructing, implementationType));
        }

        public void EndConstruct(Type implementationType)
        {
            AddItem(new LogItem(ItemType.Constructed, implementationType));
        }

        public void Reused(Type implementationType)
        {
            AddItem(new LogItem(ItemType.Reused, implementationType));
        }

        public void Crash()
        {
            wasCrash = true;
        }

        public void BeginGet(Type type)
        {
            AddItem(new LogItem(ItemType.Get, type));
        }

        public void EndGet(Type type)
        {
            AddItem(new LogItem(ItemType.EndGet, type));
        }

        public void BeginCreate(Type type)
        {
            AddItem(new LogItem(ItemType.Create, type));
        }

        public void EndCreate(Type type)
        {
            AddItem(new LogItem(ItemType.EndCreate, type));
        }

        public void BeginGetAll(Type type)
        {
            AddItem(new LogItem(ItemType.GetAll, type));
        }

        public void EndGetAll(Type type)
        {
            AddItem(new LogItem(ItemType.EndGetAll, type));
        }

        public string GetLog()
        {
            int depth = 0;
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("Container: '{0}'", containerName));
            foreach (LogItem item in items)
            {
                int delta = depthChange[(int) item.ItemType];
                if (delta < 0)
                    depth += delta;
                builder.Append(new string(' ', depth));
                item.AppendTo(builder);
                if (delta > 0)
                    depth += delta;
            }
            return builder.ToString();
        }

        #endregion
    }
}