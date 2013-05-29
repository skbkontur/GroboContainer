using System;

namespace GroboContainer.Impl.Logging
{
    public interface ILog
    {
        void BeginConstruct(Type implementationType);
        void EndConstruct(Type implementationType);

        void Reused(Type implementationType);

        void Crash();

        void BeginGet(Type type);
        void EndGet(Type type);

        void BeginCreate(Type type);
        void EndCreate(Type type);

        void BeginGetAll(Type type);
        void EndGetAll(Type type);

        string GetLog();
    }
}