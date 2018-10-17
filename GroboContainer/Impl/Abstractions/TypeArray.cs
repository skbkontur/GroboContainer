using System;

namespace GroboContainer.Impl.Abstractions
{
    public struct TypeArray : IEquatable<TypeArray>
    {
        public TypeArray(Type[] types)
        {
            this.types = types;
            hash = GetHash(types);
        }

        #region IEquatable<TypeArray> Members

        public bool Equals(TypeArray other)
        {
            if (other.types.Length != types.Length) return false;
            for (int i = 0; i < types.Length; i++)
                if (types[i] != other.types[i]) return false;
            return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is TypeArray other && Equals(other);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        private static int GetHash(Type[] types)
        {
            unchecked
            {
                int result = 0;
                for(int i = 0; i < types.Length; ++i)
                    result = (result * 397) ^ types[i].GetHashCode();
                return result;
            }
        }

        private readonly int hash;
        private readonly Type[] types;
    }
}