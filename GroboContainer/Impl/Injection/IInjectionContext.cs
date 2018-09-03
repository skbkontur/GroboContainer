using System;

using GroboContainer.Core;
using GroboContainer.Impl.Logging;

namespace GroboContainer.Impl.Injection
{
    public interface IInjectionContext
    {
        IContainer Container { get; }
        IContainerForFuncBuilder ContainerForFunc { get; }
        IInternalContainer InternalContainer { get; }

        void BeginConstruct(Type classType);
        void EndConstruct(Type classType);
        void Reused(Type classType);
        void Crash();

        void BeginGet(Type abstractionType);
        void EndGet(Type abstractionType);
        void BeginGetAll(Type abstractionType);
        void EndGetAll(Type abstractionType);

        void BeginCreate(Type abstractionType);
        void EndCreate(Type abstractionType);

        ILog GetLog();
    }
}