using System;

using GroboContainer.Infection;

namespace GroboContainer.Impl.ChildContainersSupport.Selectors
{
    public class AttributedChildSelector : IContainerSelector
    {
        #region IContainerSelector Members

        public int Select(Type abstractionType, int containerTreeDepth)
        {
            switch (containerTreeDepth)
            {
            case 0:
                if (IsChild(abstractionType))
                    throw new InvalidOperationException(string.Format("Тип {0} пытаются взять из контейнера глубины 0, а он помечен как Child",
                                                                      abstractionType));
                return 0;
            case 1:
                if (IsChild(abstractionType))
                    return 1;
                return 0;
            }
            throw new NotSupportedException("Контейнеры с глубиной больше 1 не поддерживаются");
        }

        #endregion

        private static bool IsChild(Type abstractionType)
        {
            return abstractionType.IsDefined(typeof(ChildTypeAttribute), false);
        }
    }
}