using System;

using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class RootAndChildSelector : IContainerSelector
    {
        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
            case 0:
                if (IsRoot(abstractionType))
                    return 0;
                throw new InvalidOperationException($"Type {abstractionType} is not marked as Root type");
            case 1:
                if (IsChild(abstractionType))
                    return 1;
                if (IsRoot(abstractionType))
                    return 0;
                throw new InvalidOperationException($"Type {abstractionType} is not marked as Root or Child type");
            }
            throw new NotSupportedException("Containers of depth more than 1 are not supported");
        }

        private static bool IsRoot(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof(RootTypeAttribute), false);
        }

        private static bool IsChild(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof(ChildTypeAttribute), false);
        }
    }
}