using System;

using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    public class ClassFactory : IClassFactory
    {
        public ClassFactory(Func<IInternalContainer, IInjectionContext, object[], object> creatorFunc, Type constructedType)
        {
            this.creatorFunc = creatorFunc;
            this.constructedType = constructedType;
        }

        public object Create(IInjectionContext context, object[] args)
        {
            try
            {
                context.BeginConstruct(constructedType);
                return creatorFunc(context.InternalContainer, context, args);
            }
            catch (Exception)
            {
                context.Crash();
                throw;
            }
            finally
            {
                context.EndConstruct(constructedType);
            }
        }

        private readonly Type constructedType;
        private readonly Func<IInternalContainer, IInjectionContext, object[], object> creatorFunc;
    }
}