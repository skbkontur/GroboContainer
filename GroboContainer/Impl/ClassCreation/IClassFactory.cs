using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IClassFactory
    {
        object Create(IInjectionContext context, object[] args);
    }
}