using System;

using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ChildContainersSupport.Selectors;

namespace GroboContainer.Impl.ChildContainersSupport
{
    public class CompositeCollection : IAbstractionConfigurationCollection
    {
        public CompositeCollection(IAbstractionConfigurationCollection[] rootToChildCollections,
                                   IContainerSelector selector)
        {
            if (rootToChildCollections == null || rootToChildCollections.Length == 0)
                throw new ArgumentException("rootToChildCollections");
            containerTreeDepth = rootToChildCollections.Length - 1;
            this.rootToChildCollections = rootToChildCollections;
            leafCollection = rootToChildCollections[containerTreeDepth];
            this.selector = selector;
        }

        public CompositeCollection MakeChildCollection(IAbstractionConfigurationCollection childCollection)
        {
            var newCollections = new IAbstractionConfigurationCollection[rootToChildCollections.Length + 1];
            Array.Copy(rootToChildCollections, newCollections, rootToChildCollections.Length);
            newCollections[rootToChildCollections.Length] = childCollection;
            return new CompositeCollection(newCollections, selector);
        }

        private IAbstractionConfigurationCollection ChooseCollection(Type abstractionType)
        {
            if (abstractionType == typeof(IContainer))
                return leafCollection;
            var index = selector.Select(abstractionType, containerTreeDepth);
            if (index < 0 || index > containerTreeDepth)
                throw new BadSelectorException($"Bad selector result {index}. Depth={containerTreeDepth}, Type={abstractionType}");
            return rootToChildCollections[index];
        }

        private readonly int containerTreeDepth;
        private readonly IAbstractionConfigurationCollection leafCollection;
        private readonly IAbstractionConfigurationCollection[] rootToChildCollections;
        private readonly IContainerSelector selector;

        #region IAbstractionConfigurationCollection Members

        public IAbstractionConfiguration Get(Type abstractionType)
        {
            var collection = ChooseCollection(abstractionType);
            return collection.Get(abstractionType);
        }

        public void Add(Type abstractionType, IAbstractionConfiguration abstractionConfiguration)
        {
            var collection = ChooseCollection(abstractionType);
            if (!ReferenceEquals(collection, leafCollection))
                throw new InvalidOperationException($"Тип {abstractionType} нельзя конфигурировать");
            leafCollection.Add(abstractionType, abstractionConfiguration);
        }

        public IAbstractionConfiguration[] GetAll()
        {
            //NOTE !!!
            return leafCollection.GetAll();
        }

        #endregion
    }
}