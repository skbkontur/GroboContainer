using System.Reflection;

namespace GroboContainer.Impl.ClassCreation
{
    public class ContainerConstructorInfo
    {
        public ConstructorInfo ConstructorInfo { get; set; }
        public int[] ParametersInfo { get; set; }
    }
}