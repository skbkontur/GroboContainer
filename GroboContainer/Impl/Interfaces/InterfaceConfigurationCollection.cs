using System;
using System.Collections;
using GroboContainer.Core;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Interfaces.AutoConfiguration;

namespace GroboContainer.Impl.Interfaces
{
    public class InterfaceConfigurationCollection : IInterfaceConfigurationCollection
    {
        private readonly Hashtable cache = new Hashtable();
        private readonly IAutoInterfaceConfigurationFactory factory;
        private readonly object lockObject = new object();

        public InterfaceConfigurationCollection(IAutoInterfaceConfigurationFactory factory)
        {
            this.factory = factory;
        }

        public InterfaceConfigurationCollection(IContainerConfiguration containerConfiguration)
            : this(
                new AutoInterfaceConfigurationFactory(
                    new ImplementationTypesCollection(containerConfiguration, new TypesHelper()),
                    new ImplementationConfigurationCollection(new TypesHelper())))
        {
        }

        #region IInterfaceConfigurationCollection Members

        public IInterfaceConfiguration Get(Type interfaceType, string[] requireContracts)
        {
            var key = new AbstractionKey(interfaceType, requireContracts);
            IInterfaceConfiguration result = Read(key);
            if (result == null)
                lock (lockObject)
                {
                    result = Read(key);
                    if (result == null)
                        cache.Add(key, result = factory.CreateByType(interfaceType, requireContracts));
                }
            return result;
        }

        public void Add(Type interfaceType, string[] requireContracts, IInterfaceConfiguration interfaceConfiguration)
        {
            lock (lockObject)
                cache.Add(new AbstractionKey(interfaceType, requireContracts), interfaceConfiguration);
        }

        #endregion

        private IInterfaceConfiguration Read(AbstractionKey key)
        {
            return (IInterfaceConfiguration) cache[key];
        }
    }
}