using System;

using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class AttributedRootSelector : IContainerSelector
    {
        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
            case 0:
                if (IsRoot(abstractionType))
                    return 0;
                throw new InvalidOperationException($"Type {abstractionType} cannot be obtained from container of depth 0");

            case 1:
                if (IsRoot(abstractionType))
                    return 0;
                return 1;
            }
            throw new NotSupportedException("Containers of depth more than 1 are not supported");
        }

        private static bool IsRoot(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof(RootTypeAttribute), false);
        }
    }
}