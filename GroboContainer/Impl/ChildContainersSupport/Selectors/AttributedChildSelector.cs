using System;

using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class AttributedChildSelector : IContainerSelector
    {
        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
            case 0:
                if (IsChild(abstractionType))
                    throw new InvalidOperationException($"Trying to obtain type {abstractionType} from container of depth 0, but it is marked as Child");
                return 0;
            case 1:
                if (IsChild(abstractionType))
                    return 1;
                return 0;
            }
            throw new NotSupportedException("Containers of depth more than 1 are not supported");
        }

        private static bool IsChild(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof(ChildTypeAttribute), false);
        }
    }
}