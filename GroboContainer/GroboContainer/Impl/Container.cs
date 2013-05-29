using System;
using GroboContainer.Config;
using GroboContainer.Core;

namespace GroboContainer.Impl
{
    public class Container : IContainer
    {
        private readonly IInternalContainer worker;
        public IContainerConfigurator Configurator
        {
            get { return worker.Configurator; }
        }

        public T Get<T>(params string[] requireContracts)
        {
            return worker.Get<T>(requireContracts);
        }

        public object Get(Type type, params string[] requireContracts)
        {
            return worker.Get(type, requireContracts);
        }

        public T[] GetAll<T>(params string[] requireContracts)
        {
            return worker.GetAll<T>(requireContracts);
        }

        public object[] GetAll(Type type, params string[] requireContracts)
        {
            return worker.GetAll(type, requireContracts);
        }

        public Container(IContainerConfiguration configuration)
        {
            worker = new InternalContainer(configuration);
        }
    }
}