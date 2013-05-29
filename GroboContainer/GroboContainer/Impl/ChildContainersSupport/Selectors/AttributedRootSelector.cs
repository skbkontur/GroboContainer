using System;
using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class AttributedRootSelector : IContainerSelector
    {
        #region IContainerSelector Members

        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
                case 0:
                    if (IsRoot(abstractionType))
                        return 0;
                    throw new InvalidOperationException(string.Format("Тип {0} нельзя брать из контейнера глубины 0",
                                                                      abstractionType));

                case 1:
                    if (IsRoot(abstractionType))
                        return 0;
                    return 1;
            }
            throw new NotSupportedException("Контейнеры с глубиной больше 1 не поддерживаются");
        }

        #endregion

        private static bool IsRoot(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof (RootTypeAttribute), false);
        }
    }
}