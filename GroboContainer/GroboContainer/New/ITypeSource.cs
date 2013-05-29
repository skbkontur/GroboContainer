using System;
using System.Collections.Generic;

namespace GroboContainer.New
{
    public interface ITypeSource
    {
        IEnumerable<Type> GetTypesToScan();
    }
}