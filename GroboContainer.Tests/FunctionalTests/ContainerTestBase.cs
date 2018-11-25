using System.Diagnostics;

using GroboContainer.Core;
using GroboContainer.Impl;

namespace GroboContainer.Tests.FunctionalTests
{
    public abstract class ContainerTestBase : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            configuration = new ContainerConfiguration(GetType().Assembly);
            container = new Container(configuration, new TestClassWrapperCreator());
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        protected IContainer container;
        private ContainerConfiguration configuration;
    }
}