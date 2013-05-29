namespace GroboContainer.Core
{
    public interface IContainerForFuncBuilder
    {
        T CreateForFunc<T>();
        T CreateForFunc<T1, T>(T1 arg1);
        T CreateForFunc<T1, T2, T>(T1 arg1, T2 arg2);
        T CreateForFunc<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3);
        T CreateForFunc<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        T GetForFunc<T>();
    }
}