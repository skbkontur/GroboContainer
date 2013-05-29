using System;
using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    public class FuncBuilder : IFuncBuilder
    {
        #region IFuncBuilder Members

        public Func<T> BuildGetFunc<T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Get<T>;
        }

        public Func<T> BuildCreateFunc<T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Create<T>;
        }

        public Func<T1, T> BuildCreateFunc<T1, T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Create<T1, T>;
        }

        public Func<T1, T2, T> BuildCreateFunc<T1, T2, T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Create<T1, T2, T>;
        }

        public Func<T1, T2, T3, T> BuildCreateFunc<T1, T2, T3, T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Create<T1, T2, T3, T>;
        }

        public Func<T1, T2, T3, T4, T> BuildCreateFunc<T1, T2, T3, T4, T>(IInjectionContext context)
        {
            var @class = new Context(context);
            return @class.Create<T1, T2, T3, T4, T>;
        }

        #endregion

        //TODO emit this class

        #region Nested type: Context

        private class Context
        {
            private readonly IInjectionContext context;

            public Context(IInjectionContext context)
            {
                this.context = context;
            }

            public T Get<T>()
            {
                return context.ContainerForFunc.GetForFunc<T>();
            }

            public T Create<T>()
            {
                return context.ContainerForFunc.CreateForFunc<T>();
            }

            public T Create<T1, T>(T1 arg1)
            {
                return context.ContainerForFunc.CreateForFunc<T1, T>(arg1);
            }

            public T Create<T1, T2, T>(T1 arg1, T2 arg2)
            {
                return context.ContainerForFunc.CreateForFunc<T1, T2, T>(arg1, arg2);
            }

            public T Create<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3)
            {
                return context.ContainerForFunc.CreateForFunc<T1, T2, T3, T>(arg1, arg2, arg3);
            }

            public T Create<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                return context.ContainerForFunc.CreateForFunc<T1, T2, T3, T4, T>(arg1, arg2, arg3, arg4);
            }
        }

        #endregion
    }
}