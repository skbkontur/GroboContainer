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
        public ContainerConfiguration(params Assembly[] assembliesToScan)
            : this((IEnumerable<Assembly>)assembliesToScan)
        {
        }

        public ContainerConfiguration(IEnumerable<Assembly> assembliesToScan, string name = "root", ContainerMode mode = ContainerMode.Default)
        {
            ContainerName = name;
            Mode = mode;
            var typesSet = new HashSet<Type>();
            foreach (var assembly in assembliesToScan)
            {
                Type[] typesInAssembly;
                try
                {
                    typesInAssembly = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    var sb = new StringBuilder();
                    foreach (var loaderException in e.LoaderExceptions)
                        sb.AppendLine(loaderException.Message);
                    throw new ContainerConfigurationException($"Assembly.GetTypes() failed for {assembly.FullName} (Path: {assembly.Location}):{Environment.NewLine}{sb}", e);
                }
                catch (Exception e)
                {
                    throw new ContainerConfigurationException($"Assembly.GetTypes() failed for {assembly.FullName} (Path: {assembly.Location}')", e);
                }
                foreach (var type in typesInAssembly)
                    typesSet.Add(type);
            }
            types = typesSet.ToArray();
        }

        public IEnumerable<Type> GetTypesToScan()
        {
            return types;
        }

        public string ContainerName { get; }
        public ContainerMode Mode { get; }
        private readonly Type[] types;
    }
}