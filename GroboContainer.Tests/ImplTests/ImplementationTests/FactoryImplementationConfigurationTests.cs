using System;

using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class FactoryImplementationConfigurationTests : CoreTestBase
    {
        [Test]
        public void TestDisposeCalledOnAllInstances()
        {
            var disposable1 = GetMock<ITestGeneric<int>>();
            var disposable2 = GetMock<ITestGeneric<string>>();
            var configuration = new FactoryImplementationConfiguration(typeof(ITestGeneric<>), (container, type) =>
                {
                    if (type == typeof(ITestGeneric<int>))
                        return disposable1;
                    if (type == typeof(ITestGeneric<string>))
                        return disposable2;
                    throw new AssertionException("Unexpected type requested");
                });

            var injectionContext = NewMock<IInjectionContext>();
            var creationContext = NewMock<ICreationContext>();
            injectionContext.ExpectGetContainer(null);
            configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<int>));
            injectionContext.ExpectGetContainer(null);
            configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<string>));

            disposable1.Expect(d => d.Dispose()).Repeat.Once();
            disposable2.Expect(d => d.Dispose()).Repeat.Once();

            configuration.DisposeInstance();
        }

        [Test]
        public void TestResolvedCached()
        {
            var configuration = new FactoryImplementationConfiguration(typeof(object), (container, type) => new object());
            var injectionContext = NewMock<IInjectionContext>();
            var creationContext = NewMock<ICreationContext>();
            injectionContext.ExpectGetContainer(null);
            var instance1 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(object));
            injectionContext.ExpectReused(typeof(object));
            var instance2 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(object));

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestResolvedCachedPerConcreteType()
        {
            var expectedCached1 = GetMock<ITestGeneric<int>>();
            var expectedCached2 = GetMock<ITestGeneric<string>>();
            var configuration = new FactoryImplementationConfiguration(typeof(ITestGeneric<>), (container, type) =>
                {
                    if (type == typeof(ITestGeneric<int>))
                        return expectedCached1;
                    if (type == typeof(ITestGeneric<string>))
                        return expectedCached2;
                    throw new AssertionException("Unexpected type requested");
                });

            var injectionContext = NewMock<IInjectionContext>();
            var creationContext = NewMock<ICreationContext>();
            injectionContext.ExpectGetContainer(null);
            var actualInstance1 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<int>));
            injectionContext.ExpectReused(typeof(ITestGeneric<int>));
            var actualInstance2 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<int>));
            Assert.AreSame(actualInstance1, actualInstance2);
            Assert.AreSame(expectedCached1, actualInstance1);
            
            injectionContext.ExpectGetContainer(null);
            var actualInstance3 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<string>));
            injectionContext.ExpectReused(typeof(ITestGeneric<string>));
            var actualInstance4 = configuration.GetOrCreateInstance(injectionContext, creationContext, typeof(ITestGeneric<string>));

            Assert.AreSame(actualInstance3, actualInstance4);
            Assert.AreSame(expectedCached2, actualInstance3);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface ITestGeneric<T> : IDisposable
        {
        }
    }
}