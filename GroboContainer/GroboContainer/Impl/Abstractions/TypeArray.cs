using System;

namespace GroboContainer.Impl.Abstractions
{
    public class TypeArray : IEquatable<TypeArray>
    {
        private readonly int hash;
        private readonly Type[] types;

        public TypeArray(Type[] types)
        {
            this.types = types;
            hash = GetHash();
        }

        #region IEquatable<TypeArray> Members

        public bool Equals(TypeArray other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        #endregion

        private bool IsEqual(TypeArray other)
        {
            if (other.types.Length != types.Length) return false;
            for (int i = 0; i < types.Length; i++)
                if (types[i] != other.types[i]) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TypeArray)) return false;
            return Equals((TypeArray) obj);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        private int GetHash()
        {
            unchecked
            {
                int result = 0;
                foreach (Type type in types)
                    result = (result*397) ^ type.GetHashCode();
                return result;
            }
        }
    }
}