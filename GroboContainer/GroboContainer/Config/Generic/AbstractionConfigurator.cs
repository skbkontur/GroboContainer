using System.Linq;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;

namespace GroboContainer.Config.Generic
{
    public class AbstractionConfigurator<T> : IAbstractionConfigurator<T>
    {
        private readonly AbstractionConfigurator worker;

        public AbstractionConfigurator(IAbstractionConfigurationCollection abstractionConfigurationCollection, IClassWrapperCreator classWrapperCreator)
        {
            worker = new AbstractionConfigurator(typeof (T), abstractionConfigurationCollection, classWrapperCreator);
        }

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