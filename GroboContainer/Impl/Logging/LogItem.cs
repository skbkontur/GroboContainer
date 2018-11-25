using System;
using System.Text;

namespace GroboContainer.Impl.Logging
{
    public struct LogItem
    {
        public LogItem(ItemType itemType, Type type)
        {
            ItemType = itemType;
            this.type = type;
        }

        public ItemType ItemType { get; }

        public override string ToString()
        {
            return $"{ItemType}<{type.Name}>";
        }

        public void AppendTo(StringBuilder builder)
        {
            builder.AppendFormat("{0}<{1}>(", ItemType, type);
            builder.AppendLine(")");
        }

        private readonly Type type;
    }
}