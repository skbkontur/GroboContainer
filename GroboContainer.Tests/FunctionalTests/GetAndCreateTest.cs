using System;
using GroboContainer.Core;
using GroboContainer.Impl;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class GetAndCreateTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            configuration = new ContainerConfiguration(new[] {GetType().Assembly});
            container = new Container(configuration);
        }

        private ContainerConfiguration configuration;
        private Container container;

        private interface IClassReuseNever
        {
        }

        private class ClassReuseNever : IClassReuseNever
        {
        }

        private class ClassWithArgumentsReuseNever
        {
            public ClassWithArgumentsReuseNever(Func<IClassReuseNever> createReuseNever)
            {
                Dependency = createReuseNever();
            }

            public IClassReuseNever Dependency { get; private set; }
        }

        private class ClassWithArgumentsReuseAllways
        {
            public ClassWithArgumentsReuseAllways(ClassReuseAllways reuseAllways)
            {
                Dependency = reuseAllways;
            }

            public ClassReuseAllways Dependency { get; private set; }
        }

        public class ClassReuseAllways
        {
        }

        [Test]
        public void TestGetReuseesInstance()
        {
            var instance1 = container.Get<ClassReuseAllways>();
            var instance2 = container.Get<ClassReuseAllways>();
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestReuseDependenciesReuseAllways()
        {
            var instance1 = container.Get<ClassWithArgumentsReuseAllways>();
            var instance2 = container.Get<ClassWithArgumentsReuseAllways>();
            Assert.AreSame(instance1.Dependency, instance2.Dependency);
        }

        [Test]
        public void TestReuseDependenciesReuseNever()
        {
            var instance1 = (ClassWithArgumentsReuseNever) container.Create(typeof (ClassWithArgumentsReuseNever));
            var instance2 = (ClassWithArgumentsReuseNever) container.Create(typeof (ClassWithArgumentsReuseNever));
            Assert.AreNotSame(instance1.Dependency, instance2.Dependency);
        }

        [Test]
        public void TestReuseNever()
        {
            var instance1 = container.Create<IClassReuseNever>();
            var instance2 = container.Create<IClassReuseNever>();
            Assert.That(instance1, Is.InstanceOf<ClassReuseNever>());
            Assert.AreNotSame(instance1, instance2);
        }
    }
}