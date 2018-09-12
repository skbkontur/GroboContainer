using System.Linq;

using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.New;

namespace GroboContainer.Config.Generic
{
    public class AbstractionConfigurator<T> : IAbstractionConfigurator<T>
    {
        public AbstractionConfigurator(IAbstractionConfigurationCollection abstractionConfigurationCollection,
                                       IClassWrapperCreator classWrapperCreator, IImplementationConfigurationCache implementationConfigurationCache,
                                       IImplementationCache implementationCache)
        {
            worker = new AbstractionConfigurator(typeof(T), abstractionConfigurationCollection, classWrapperCreator,
                                                 implementationConfigurationCache, implementationCache);
        }

        private readonly AbstractionConfigurator worker;

        #region IAbstractionConfigurator<T> Members

        public void UseType<TImpl>() where TImpl : T
        {
            worker.UseType(typeof(TImpl));
        }

        public void UseInstances(params T[] instances)
        {
            object[] objects = instances.Cast<object>().ToArray();
            worker.UseInstances(objects);
        }

        public void Fail()
        {
            worker.Fail();
        }

        #endregion
    }
}