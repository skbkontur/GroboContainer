using System;
using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class RootAndChildSelector : IContainerSelector
    {
        #region IContainerSelector Members

        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
                case 0:
                    if (IsRoot(abstractionType))
                        return 0;
                    throw new InvalidOperationException(string.Format("Type {0} not marked as Root type",
                                                                      abstractionType));
                case 1:
                    if (IsChild(abstractionType))
                        return 1;
                    if (IsRoot(abstractionType))
                        return 0;
                    throw new InvalidOperationException(string.Format("Type {0} not marked as Root or Child type",
                                                                      abstractionType));
            }
            throw new NotSupportedException("Контейнеры с глубиной больше 1 не поддерживаются");
        }

        #endregion

        private static bool IsRoot(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof (RootTypeAttribute), false);
        }

        private static bool IsChild(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof (ChildTypeAttribute), false);
        }
    }
}