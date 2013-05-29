using System;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public interface IContainerSelector
    {
        int Select(Type abstractionType, int containerTreeDepth);
    }
}