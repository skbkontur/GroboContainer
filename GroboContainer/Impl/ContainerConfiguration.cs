using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GroboContainer.Core;

namespace GroboContainer.Impl
{
    public class ContainerConfiguration : IContainerConfiguration
    {
        private readonly Type[] types;

        public ContainerConfiguration(params Assembly[] assembliesToScan)
            : this((IEnumerable<Assembly>) assembliesToScan)
        {
        }

        public ContainerConfiguration(IEnumerable<Assembly> assembliesToScan, string name = "root",
                                      ContainerMode mode = ContainerMode.Default)
        {
            ContainerName = name;
            Mode = mode;
            var typesSet = new HashSet<Type>();
            foreach (Assembly assembly in assembliesToScan)
            {
                Type[] typesInAssembly;
                try
                {
                    typesInAssembly = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    var sb = new StringBuilder();
                    foreach (Exception loaderException in e.LoaderExceptions)
                        sb.AppendLine(loaderException.Message);
                    throw new ContainerConfigurationException(
                        string.Format("Ошибка при получении типов из сборки '{0}'\r\n(Path:'{1}')\n{2}",
                                      assembly.FullName, assembly.Location, sb), e);
                }
                catch (Exception e)
                {
                    throw new ContainerConfigurationException(
                        string.Format("Ошибка при получении типов из сборки '{0}'\r\n(Path:'{1}')",
                                      assembly.FullName, assembly.Location), e);
                }
                foreach (Type type in typesInAssembly)
                    typesSet.Add(type);
            }
            types = typesSet.ToArray();
        }


        public IEnumerable<Type> GetTypesToScan()
        {
            return types;
        }

        public string ContainerName { get; private set; }
        public ContainerMode Mode { get; private set; }
    }
}