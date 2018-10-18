using System;
using System.Collections.Generic;
using System.Text;

namespace GroboContainer.Impl.Logging
{
    public class ShortGroboContainerLog : IGroboContainerLog
    {
        static ShortGroboContainerLog()
        {
            depthChange = LogSettings.depthChange;
        }

        public ShortGroboContainerLog(string containerName)
        {
            this.containerName = containerName;
        }

        private void AddItem(ItemType itemType, Type type)
        {
            if (wasCrash)
                return;
            int add = depthChange[(int)itemType];
            switch (add)
            {
                case +1:
                    items.Add(new LogItem(itemType, type));
                    break;
                case -1:
                    items.RemoveAt(items.Count - 1);
                    break;
                case 0:
                    break;
                default:
                    throw new NotSupportedException(string.Format("bad item {0}", itemType));
            }
        }

        private readonly string containerName;
        private static readonly int[] depthChange;
        private readonly List<LogItem> items = new List<LogItem>();
        private bool wasCrash;

        #region IGroboContainerLog Members

        public void BeginConstruct(Type implementationType)
        {
            AddItem(ItemType.Constructing, implementationType);
        }

        public void EndConstruct(Type implementationType)
        {
            AddItem(ItemType.Constructed, implementationType);
        }

        public void Reused(Type implementationType)
        {
            AddItem(ItemType.Reused, implementationType);
        }

        public void Crash()
        {
            wasCrash = true;
        }

        public void BeginGet(Type type)
        {
            AddItem(ItemType.Get, type);
        }

        public void EndGet(Type type)
        {
            AddItem(ItemType.EndGet, type);
        }

        public void BeginCreate(Type type)
        {
            AddItem(ItemType.Create, type);
        }

        public void EndCreate(Type type)
        {
            AddItem(ItemType.EndCreate, type);
        }

        public void BeginGetAll(Type type)
        {
            AddItem(ItemType.GetAll, type);
        }

        public void EndGetAll(Type type)
        {
            AddItem(ItemType.EndGetAll, type);
        }

        public string GetLog()
        {
            int depth = 0;
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("Container: '{0}'", containerName));
            foreach (LogItem item in items)
            {
                int delta = depthChange[(int)item.ItemType];
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