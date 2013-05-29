using System;
using System.Text;

namespace GroboContainer.Impl.Logging
{
    public class LogItem
    {
        private readonly Type type;


        public LogItem(ItemType itemType, Type type)
        {
            ItemType = itemType;
            this.type = type;
        }

        public ItemType ItemType { get; private set; }


        public void AppendTo(StringBuilder builder)
        {
            builder.AppendFormat("{0}<{1}>(", ItemType, type);
            builder.Append(")\r\n");
        }
    }
}