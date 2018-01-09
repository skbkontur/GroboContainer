using System;
using GroboContainer.Impl.Abstractions;

namespace GroboContainer.OldHlam
{
    public class AbstractionKey : IEquatable<AbstractionKey>
    {
        private readonly string contracts;
        private readonly int hash;
        private readonly Type type;

        public AbstractionKey(Type type, string[] requireContracts)
        {
            this.type = type;
            contracts = requireContracts.ConvertToString();
            hash = GetHash();
        }

        #region IEquatable<AbstractionKey> Members

        public bool Equals(AbstractionKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.type == type && Equals(other.contracts, contracts);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AbstractionKey)) return false;
            return Equals((AbstractionKey) obj);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        private int GetHash()
        {
            unchecked
            {
                return ((type != null ? type.GetHashCode() : 0)*397) ^ (contracts != null ? contracts.GetHashCode() : 0);
            }
        }
    }
}