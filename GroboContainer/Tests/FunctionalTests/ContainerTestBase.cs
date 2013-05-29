using System.Diagnostics;
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
            container = new Container(configuration);
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        #endregion

        private ContainerConfiguration configuration;
        protected IContainer container;
    }
}