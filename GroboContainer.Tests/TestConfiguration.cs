using System;
using System.Collections.Generic;

using GroboContainer.Core;

namespace GroboContainer.Tests
{
    public class TestConfiguration : IContainerConfiguration
    {
        public TestConfiguration(IEnumerable<Type> types)
        {
            this.types = types;
        }

        #region IContainerConfiguration Members

        public IEnumerable<Type> GetTypesToScan()
        {
            return types;
        }

        #endregion

        public string ContainerName { get { return "Test"; } }

        public ContainerMode Mode { get { return ContainerMode.Default; } }

        private readonly IEnumerable<Type> types;
    }
}