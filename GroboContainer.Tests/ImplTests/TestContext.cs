using System;
using GroboContainer.Config;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;

namespace Tests.ImplTests
{
	public class TestContext : IContainerContext
	{
		public IContainerConfigurator ContainerConfigurator { get; set; }
		public IContainerConfiguration Configuration { get; set; }

		#region IContainerContext Members

		public IFuncBuilder FuncBuilder { get; set; }

		public ICreationContext CreationContext { get; set; }

		public IAbstractionConfigurationCollection AbstractionConfigurationCollection { get; set; }
		public IImplementationConfigurationCache ImplementationConfigurationCache { get; set; }
		public IImplementationCache ImplementationCache { get; set; }

		public IClassWrapperCreator ClassWrapperCreator { get; set; }

		public IContainerContext MakeChildContext()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}