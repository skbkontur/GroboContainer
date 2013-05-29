using System.Diagnostics;

using GroboContainer;
using GroboContainer.Core;
using GroboContainer.Impl;

namespace Tests.FunctionalTests
{
    public abstract class ContainerTestBase : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            configuration = new ContainerConfiguration(new[] {GetType().Assembly});
            container = new Container(configuration, new TestClassWrapperCreator());
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        #endregion

        protected IContainer container;
        private ContainerConfiguration configuration;
    }
}