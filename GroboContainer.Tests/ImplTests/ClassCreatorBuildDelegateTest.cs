using System;
using System.Linq;

using GroboContainer.Impl;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class ClassCreatorBuildDelegateTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            classCreator = new ClassCreator(new FuncHelper());
            internalContainerMock = GetMock<IInternalContainer>();
            contextMock = GetMock<IInjectionContext>();
        }

        public interface I1
        {
        }

        public interface I2
        {
        }

        private void DoTestWithParameters(object[] objects)
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i2Mock = GetMock<I2>().Object;
            internalContainerMock.Setup(x => x.Get<I2>(contextMock.Object)).Returns(i2Mock);
            contextMock.Setup(c => c.BeginConstruct(typeof(C3)));
            contextMock.Setup(c => c.EndConstruct(typeof(C3)));
            var factoryConstructorInfo = new ContainerConstructorInfo
                {
                    ConstructorInfo =
                        typeof(C3).GetConstructor(new[]
                            {
                                typeof(int), typeof(I2),
                                typeof(string)
                            }),
                    ParametersInfo = new[] {1, -1, 0}
                };

            var classFactory = classCreator.BuildFactory(factoryConstructorInfo, null);
            var instance = classFactory.Create(contextMock.Object, objects);
            Assert.That(instance, Is.InstanceOf<C3>());
            Assert.AreSame(i2Mock, ((C3)instance).z);
            Assert.AreSame(objects[0], ((C3)instance).b);
            Assert.AreEqual(objects[1], ((C3)instance).a);
        }

        private void DoTestWithParametersBig(object[] objects)
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = GetMock<I1>().Object;
            internalContainerMock.Setup(x => x.Get<I1>(contextMock.Object)).Returns(i1Mock);
            contextMock.Setup(c => c.BeginConstruct(typeof(C3)));
            contextMock.Setup(c => c.EndConstruct(typeof(C3)));
            var factoryConstructorInfo = new ContainerConstructorInfo
                {
                    ConstructorInfo =
                        typeof(C3).GetConstructor(new[]
                            {
                                typeof(int[]),
                                typeof(object[]), typeof(I1),
                                typeof(int?), typeof(object)
                            }),
                    ParametersInfo = new[] {3, 1, -1, 0, 2}
                };

            var classFactory = classCreator.BuildFactory(factoryConstructorInfo, null);
            var instance = classFactory.Create(contextMock.Object, objects);
            Assert.That(instance, Is.InstanceOf<C3>());
            Assert.AreSame(i1Mock, ((C3)instance).i1Field);
            Assert.AreEqual(objects[0], ((C3)instance).intField);
            Assert.AreEqual(objects[1], ((C3)instance).objectArrayField);
            Assert.AreEqual(objects[2], ((C3)instance).objectField);
            Assert.AreEqual(objects[3], ((C3)instance).intArrayField);
        }

        [Test]
        public void TestArray()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = new[] {GetMock<I1>(), GetMock<I1>()}.Select(x => x.Object).ToArray();
            internalContainerMock.Setup(x => x.GetAll<I1>(contextMock.Object)).Returns(i1Mock);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));

            var classFactory = classCreator.BuildFactory(new ContainerConstructorInfo
                {
                    ConstructorInfo = testType.GetConstructor(new[] {typeof(I1[])})
                }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));
            Assert.AreSame(i1Mock, ((I2Impl)instance).i1Array);
        }

        [Test]
        public void TestBadFunc()
        {
            var constructorInfo = new ContainerConstructorInfo
                {
                    ConstructorInfo = testType.GetConstructor(new[] {typeof(Func<I1>), typeof(long)})
                };
            RunMethodWithException<NotSupportedException>(() => classCreator.BuildFactory(constructorInfo, wrapperType : null));
        }

        [Test]
        public void TestCrashWhenValueObjectIsNull()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var objects = new object[] {null};
            var factoryConstructorInfo = new ContainerConstructorInfo
                {
                    ConstructorInfo = typeof(C3).GetConstructor(new[]
                        {
                            typeof(int)
                        }),
                    ParametersInfo = new[] {0}
                };
            contextMock.Setup(c => c.BeginConstruct(typeof(C3)));
            contextMock.Setup(c => c.EndConstruct(typeof(C3)));
            contextMock.Setup(x => x.Crash());
            var classFactory = classCreator.BuildFactory(factoryConstructorInfo, null);
            RunMethodWithException<ArgumentException>(() => classFactory.Create(contextMock.Object, objects), "bad parameter");
        }

        [Test]
        public void TestCreateFunc()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = new I1[1];
            Func<I1> func = () => i1Mock[0];
            internalContainerMock.Setup(x => x.BuildCreateFunc<I1>(contextMock.Object)).Returns(func);
            internalContainerMock.Setup(x => x.Get<int>(contextMock.Object)).Returns(1);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof(Func<I1>), typeof(int)})
                    }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));

            Assert.AreSame(func, ((I2Impl)instance).i1Func);
            i1Mock[0] = GetMock<I1>().Object;
            Assert.AreSame(i1Mock[0], ((I2Impl)instance).i1Func());
        }

        [Test]
        public void TestGet()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = GetMock<I1>().Object;
            internalContainerMock.Setup(x => x.Get<I1>(contextMock.Object)).Returns(i1Mock);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof(I1)})
                    }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));
            Assert.AreSame(i1Mock, ((I2Impl)instance).i1);
        }

        [Test]
        public void TestWithWrap()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = GetMock<I1>().Object;
            internalContainerMock.Setup(x => x.Get<I1>(contextMock.Object)).Returns(i1Mock);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof(I1)})
                    }, typeof(I2ImplWrapper));
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf<I2ImplWrapper>());
            Assert.AreSame(i1Mock, ((I2ImplWrapper)instance).I1);
        }

        [Test]
        public void TestClassFactoryWorks()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = GetMock<I1>().Object;
            internalContainerMock.Setup(x => x.Get<I1>(contextMock.Object)).Returns(i1Mock);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof(I1)})
                    }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));
            Assert.AreSame(i1Mock, ((I2Impl)instance).i1);
        }

        [Test]
        public void TestGetFunc()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = new I1[1];
            Func<I1> func = () => i1Mock[0];
            internalContainerMock.Setup(x => x.BuildGetFunc<I1>(contextMock.Object)).Returns(func);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory =
                classCreator.BuildFactory(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = testType.GetConstructor(new[] {typeof(Func<I1>)})
                        }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));

            Assert.AreSame(func, ((I2Impl)instance).i1Func);
            i1Mock[0] = GetMock<I1>().Object;
            Assert.AreSame(i1Mock[0], ((I2Impl)instance).i1Func());
        }

        [Test]
        public void TestGetWithRequireContracts()
        {
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);

            var i1Mock = GetMock<I1>().Object;
            internalContainerMock.Setup(x => x.Get<I1>(contextMock.Object)).Returns(i1Mock);
            internalContainerMock.Setup(x => x.Get<int>(contextMock.Object)).Returns(5);
            contextMock.Setup(c => c.BeginConstruct(testType));
            contextMock.Setup(c => c.EndConstruct(testType));
            var classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof(I1), typeof(int)})
                    }, null);
            var instance = classFactory.Create(contextMock.Object, new object[0]);
            Assert.That(instance, Is.InstanceOf(testType));
            Assert.AreSame(i1Mock, ((I2Impl)instance).i1);
        }

        [Test]
        public void TestWithParameters()
        {
            DoTestWithParameters(new object[] {"zz", 123});
        }

        [Test]
        public void TestWithParametersBig()
        {
            DoTestWithParametersBig(new[] {8989, new object[] {"e", 66}, new object(), new[] {1, 2}});
        }

        [Test]
        public void TestWithParametersBigDefaultValues()
        {
            DoTestWithParametersBig(new object[] {null, null, null, null});
        }

        [Test]
        public void TestWithParametersDefaultValue()
        {
            DoTestWithParameters(new object[] {null, 123});
        }

        private ClassCreator classCreator;
        private Mock<IInternalContainer> internalContainerMock;
        private Mock<IInjectionContext> contextMock;
        private readonly Type testType = typeof(I2Impl);

        private class I2Impl : I2
        {
            public I2Impl(I1 i1)
            {
                this.i1 = i1;
            }

            public I2Impl(I1 i1, int a)
            {
                this.i1 = i1;
            }

            public I2Impl(I1[] i1)
            {
                i1Array = i1;
            }

            public I2Impl(Func<I1> getI1Func)
            {
                i1Func = getI1Func;
            }

            public I2Impl(Func<I1> createI1Func, int z)
            {
                i1Func = createI1Func;
            }

            public I2Impl(Func<I1> badcreateFunc, long z)
            {
            }

            public readonly I1 i1;
            public readonly I1[] i1Array;
            public readonly Func<I1> i1Func;
        }

        private class I2ImplWrapper
        {
            public I2ImplWrapper(object impl)
            {
                this.impl = (I2Impl)impl;
            }

            public I1 I1 => impl.i1;
            private readonly I2Impl impl;
        }

        private class C3
        {
            public C3(int a, I2 z, string b)
            {
                this.a = a;
                this.z = z;
                this.b = b;
            }

            public C3(int a)
            {
            }

            public C3(int[] intArrayField, object[] objectArrayField, I1 i1Field, int? intField, object objectField)
            {
                this.intArrayField = intArrayField;
                this.objectArrayField = objectArrayField;
                this.i1Field = i1Field;
                this.intField = intField;
                this.objectField = objectField;
            }

            public readonly int a;
            public readonly string b;
            public readonly I1 i1Field;
            public readonly int[] intArrayField;
            public readonly int? intField;
            public readonly object[] objectArrayField;
            public readonly object objectField;

            public readonly I2 z;
        }
    }
}