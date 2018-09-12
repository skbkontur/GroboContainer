namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    //public class AutoImplementationConfigurationTest : CoreTestBase
    //{
    //    #region Setup/Teardown

    //    public override void SetUp()
    //    {
    //        base.SetUp();
    //        context = NewMock<IInjectionContext>();
    //        internalContainer = NewMock<IInternalContainer>();
    //        creator = NewMock<IClassCreator>();
    //        constructorSelector = NewMock<IConstructorSelector>();
    //        type = typeof (int);
    //        info = new ContainerConstructorInfo();
    //        provider = NewMock<IImplementationFactoryProvider>();
    //        Assert.IsNotNull(info);
    //    }

    //    #endregion

    //    private IInjectionContext context;
    //    private IInternalContainer internalContainer;
    //    private IClassCreator creator;
    //    private IConstructorSelector constructorSelector;
    //    private Type type;
    //    private ContainerConstructorInfo info;
    //    private IImplementationFactoryProvider provider;

    //    [Test]
    //    public void TestDisposeNothing()
    //    {
    //        type = typeof (IDisposable);
    //        var configuration = new AutoImplementationConfiguration(type, creator, constructorSelector);

    //        configuration.DisposeInstance();
    //    }

    //    [Test]
    //    public void TestGetFactory()
    //    {
    //        var configuration = new AutoImplementationConfiguration(type, creator, constructorSelector);

    //        var factory1 = NewMock<IImplementationFactory>();
    //        provider.ExpectGet(type, Type.EmptyTypes, factory1);
    //        Assert.AreSame(factory1, configuration.GetFactory(Type.EmptyTypes, provider));
    //        mockery.VerifyAllExpectationsHaveBeenMet();
    //        Assert.AreSame(factory1, configuration.GetFactory(Type.EmptyTypes, provider));

    //        var factory2 = NewMock<IImplementationFactory>();
    //        provider.ExpectGet(type, new[] {typeof (string), typeof (Guid)}, factory2);
    //        Assert.AreSame(factory2, configuration.GetFactory(new[] {typeof (string), typeof (Guid)}, provider));
    //        mockery.VerifyAllExpectationsHaveBeenMet();
    //        Assert.AreSame(factory2, configuration.GetFactory(new[] {typeof (string), typeof (Guid)}, provider));
    //    }

    //    [Test]
    //    public void TestGetInstanceAndDisposeIt()
    //    {
    //        IDisposable disposable = null;
    //        type = typeof (IDisposable);
    //        Func<IInternalContainer, IInjectionContext, object[], object> func =
    //            (aInternalContainer, injectionContext, objs) =>
    //                {
    //                    CollectionAssert.IsEmpty(objs);
    //                    Assert.AreSame(context, injectionContext);
    //                    Assert.AreSame(internalContainer, aInternalContainer);
    //                    Assert.IsNull(disposable);
    //                    return disposable = NewMock<IDisposable>();
    //                };
    //        constructorSelector.ExpectGetConstructor(type, Type.EmptyTypes, info);
    //        creator.ExpectBuildConstructionDelegate(info, func);

    //        var configuration = new AutoImplementationConfiguration(type, creator, constructorSelector);

    //        context.ExpectGetInternalContainer(internalContainer);
    //        context.ExpectBeginConstruct(type);
    //        context.ExpectEndConstruct(type);

    //        object instance = configuration.GetOrCreateInstance(context, zzz);
    //        Assert.AreSame(disposable, instance);

    //        context.ExpectReused(type);
    //        Assert.AreSame(disposable, configuration.GetOrCreateInstance(context, zzz));

    //        disposable.ExpectDispose();
    //        configuration.DisposeInstance();
    //    }

    //    [Test]
    //    public void TestNonDisposable()
    //    {
    //        type = typeof (int);
    //        Func<IInternalContainer, IInjectionContext, object[], object> func =
    //            (aInternalContainer, injectionContext, objs) =>
    //                {
    //                    Assert.AreSame(context, injectionContext);
    //                    Assert.AreSame(internalContainer, aInternalContainer);
    //                    return 1;
    //                };
    //        constructorSelector.ExpectGetConstructor(type, Type.EmptyTypes, info);
    //        creator.ExpectBuildConstructionDelegate(info, func);

    //        var configuration = new AutoImplementationConfiguration(type, creator, constructorSelector);

    //        context.ExpectGetInternalContainer(internalContainer);
    //        context.ExpectBeginConstruct(type);
    //        context.ExpectEndConstruct(type);

    //        object instance = configuration.GetOrCreateInstance(context, zzz);
    //        Assert.AreEqual(1, instance);

    //        configuration.DisposeInstance();
    //    }
    //}
}