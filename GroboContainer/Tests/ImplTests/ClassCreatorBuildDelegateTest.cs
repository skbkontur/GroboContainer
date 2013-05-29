using System;
using GroboContainer.Impl;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using TestCore;
using Rhino.Mocks;

namespace Tests.ImplTests
{
    public class ClassCreatorBuildDelegateTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            classCreator = new ClassCreator(new FuncHelper());
            internalContainer = NewMock<IInternalContainer>();
            context = GetMock<IInjectionContext>();
        }

        #endregion

        private ClassCreator classCreator;
        private IInternalContainer internalContainer;
        private IInjectionContext context;
        private readonly Type testType = typeof (I2Impl);

        // ReSharper disable UnusedMember.Local
        public interface I1
        {
        }

        public interface I2
        {
        }

        private class I2Impl : I2
        {
            public readonly I1 i1;
            public readonly I1[] i1Array;
            public readonly Func<I1> i1Func;

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
        }

        private class C3
        {
            public readonly int a;
            public readonly string b;
            public readonly I1 i1Field;
            public readonly int[] intArrayField;
            public readonly int? intField;
            public readonly object[] objectArrayField;
            public readonly object objectField;


            public readonly I2 z;

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
        }

        // ReSharper restore UnusedMember.Local

        private void DoTestWithParameters(object[] objects)
        {
            var i2Mock = NewMock<I2>();
            internalContainer.ExpectGet(context, i2Mock);

            var factoryConstructorInfo = new ContainerConstructorInfo
                                             {
                                                 ConstructorInfo =
                                                     typeof (C3).GetConstructor(new[]
                                                                                    {
                                                                                        typeof (int), typeof (I2),
                                                                                        typeof (string)
                                                                                    }),
                                                 ParametersInfo = new[] {1, -1, 0}
                                             };

            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(factoryConstructorInfo);
            object instance = @delegate(internalContainer, context, objects);
            Assert.IsInstanceOfType(typeof (C3), instance);
            Assert.AreSame(i2Mock, ((C3) instance).z);
            Assert.AreSame(objects[0], ((C3) instance).b);
            Assert.AreEqual(objects[1], ((C3) instance).a);
        }

        private void DoTestWithParametersBig(object[] objects)
        {
            var i1Mock = NewMock<I1>();
            internalContainer.ExpectGet(context, i1Mock);

            var factoryConstructorInfo = new ContainerConstructorInfo
                                             {
                                                 ConstructorInfo =
                                                     typeof (C3).GetConstructor(new[]
                                                                                    {
                                                                                        typeof (int[]),
                                                                                        typeof (object[]), typeof (I1),
                                                                                        typeof (int?), typeof (object)
                                                                                    }),
                                                 ParametersInfo = new[] {3, 1, -1, 0, 2}
                                             };

            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(factoryConstructorInfo);
            object instance = @delegate(internalContainer, context, objects);
            Assert.IsInstanceOfType(typeof (C3), instance);
            Assert.AreSame(i1Mock, ((C3) instance).i1Field);
            Assert.AreEqual(objects[0], ((C3) instance).intField);
            Assert.AreEqual(objects[1], ((C3) instance).objectArrayField);
            Assert.AreEqual(objects[2], ((C3) instance).objectField);
            Assert.AreEqual(objects[3], ((C3) instance).intArrayField);
        }

        [Test]
        public void TestArray()
        {
            var i1Mock = new[] {NewMock<I1>(), NewMock<I1>()};
            internalContainer.ExpectGetAll(context, i1Mock);
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(new ContainerConstructorInfo
                                                           {
                                                               ConstructorInfo =
                                                                   testType.GetConstructor(new[] {typeof (I1[])})
                                                           });
            object instance = @delegate(internalContainer, context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);
            Assert.AreSame(i1Mock, ((I2Impl) instance).i1Array);
        }

        [Test]
        public void TestBadFunc()
        {
            RunMethodWithException<NotSupportedException>(
                () =>
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo =
                                testType.GetConstructor(new[] {typeof (Func<I1>), typeof (long)})
                        }));
        }


        [Test]
        public void TestCrashWhenValueObjectIsNull()
        {
            var objects = new object[] {null};
            var factoryConstructorInfo = new ContainerConstructorInfo
                                             {
                                                 ConstructorInfo =
                                                     typeof (C3).GetConstructor(new[]
                                                                                    {
                                                                                        typeof (int)
                                                                                    }),
                                                 ParametersInfo = new[] {0}
                                             };

            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(factoryConstructorInfo);
            RunMethodWithException<ArgumentException>(() => @delegate(internalContainer, context, objects), "bad parameter");
        }

        [Test]
        public void TestCreateFunc()
        {
            var i1Mock = new I1[1];
            Func<I1> func = () => i1Mock[0];
            internalContainer.ExpectBuildCreateFunc(context, func);
            internalContainer.ExpectGet(context, 1);
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo =
                                testType.GetConstructor(new[] {typeof (Func<I1>), typeof (int)})
                        });
            object instance = @delegate(internalContainer, context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);

            Assert.AreSame(func, ((I2Impl) instance).i1Func);
            i1Mock[0] = NewMock<I1>();
            Assert.AreSame(i1Mock[0], ((I2Impl) instance).i1Func());
        }

        [Test]
        public void TestGet()
        {
            var i1Mock = NewMock<I1>();
            internalContainer.ExpectGet(context, i1Mock);
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = testType.GetConstructor(new[] {typeof (I1)})
                        });
            object instance = @delegate(internalContainer, context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);
            Assert.AreSame(i1Mock, ((I2Impl) instance).i1);
        }

        [Test]
        public void TestClassFactoryWorks()
        {
            var i1Mock = NewMock<I1>();
            context.Expect(c => c.InternalContainer).Return(internalContainer);
            internalContainer.ExpectGet(context, i1Mock);

            IClassFactory classFactory = classCreator.BuildFactory(
                new ContainerConstructorInfo
                    {
                        ConstructorInfo = testType.GetConstructor(new[] {typeof (I1)})
                    });
            context.Expect(c => c.BeginConstruct(testType));
            context.Expect(c => c.EndConstruct(testType));
            object instance = classFactory.Create(context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);
            Assert.AreSame(i1Mock, ((I2Impl) instance).i1);
        }

        [Test]
        public void TestGetFunc()
        {
            var i1Mock = new I1[1];
            Func<I1> func = () => i1Mock[0];
            internalContainer.ExpectBuildGetFunc(context, func);
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = testType.GetConstructor(new[] {typeof (Func<I1>)})
                        });
            object instance = @delegate(internalContainer, context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);

            Assert.AreSame(func, ((I2Impl) instance).i1Func);
            i1Mock[0] = NewMock<I1>();
            Assert.AreSame(i1Mock[0], ((I2Impl) instance).i1Func());
        }

        [Test]
        public void TestGetWithRequireConracts()
        {
            var i1Mock = NewMock<I1>();
            internalContainer.ExpectGet(context, i1Mock);
            internalContainer.ExpectGet(context, 5);
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = testType.GetConstructor(new[] {typeof (I1), typeof (int)})
                        });
            object instance = @delegate(internalContainer, context, new object[0]);
            Assert.IsInstanceOfType(testType, instance);
            Assert.AreSame(i1Mock, ((I2Impl) instance).i1);
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
    }
}