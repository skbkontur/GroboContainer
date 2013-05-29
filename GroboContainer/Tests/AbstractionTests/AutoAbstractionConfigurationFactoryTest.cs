using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;
using NUnit.Framework;
using Rhino.Mocks;
using TestCore;
using Tests.TypesHelperTests;

namespace Tests.AbstractionTests
{
    public class AutoAbstractionConfigurationFactoryTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            typesHelper = NewMock<ITypesHelper>();
            abstractionsCollection = GetMock<IAbstractionsCollection>();
            implementationConfigurationCache = GetMock<IImplementationConfigurationCache>();
            factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                              implementationConfigurationCache);
        }

        #endregion

        private ITypesHelper typesHelper;
        private AutoAbstractionConfigurationFactory factory;
        private IAbstractionsCollection abstractionsCollection;
        private IImplementationConfigurationCache implementationConfigurationCache;

        [Test]
        public void TestIgnoredAbstraction()
        {
            typesHelper.ExpectIsIgnoredAbstraction(typeof (int), true);
            IAbstractionConfiguration configuration = factory.CreateByType(typeof (int));
            Assert.IsInstanceOfType(typeof (StupidAbstractionConfiguration), configuration);
            IImplementationConfiguration[] implementations = configuration.GetImplementations();
            CollectionAssert.AllItemsAreInstancesOfType(implementations, typeof (ForbiddenImplementationConfiguration));
        }

        [Test]
        public void TestOk()
        {
            typesHelper.ExpectIsIgnoredAbstraction(typeof (int), false);
            var abstraction = GetMock<IAbstraction>();
            abstractionsCollection.Expect(collection => collection.Get(typeof (int))).Return(abstraction);
            var implementations = new[] {GetMock<IImplementation>(), GetMock<IImplementation>()};
            var implementationConfigs = new[]
                                            {
                                                GetMock<IImplementationConfiguration>(),
                                                GetMock<IImplementationConfiguration>()
                                            };
            abstraction.Expect(abstraction1 => abstraction1.GetImplementations()).Return(implementations);

            implementationConfigurationCache.Expect(icc => icc.GetOrCreate(implementations[0])).Return(
                implementationConfigs[0]);
            implementationConfigurationCache.Expect(icc => icc.GetOrCreate(implementations[1])).Return(
                implementationConfigs[1]);

            IAbstractionConfiguration configuration = factory.CreateByType(typeof (int));
            Assert.IsInstanceOfType(typeof (AutoAbstractionConfiguration), configuration);
            IImplementationConfiguration[] configurations = configuration.GetImplementations();
            CollectionAssert.AreEqual(implementationConfigs, configurations);
        }
    }
}