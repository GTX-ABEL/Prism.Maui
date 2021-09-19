using System;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Unity.Extensions;

namespace Prism.Unity
{
    partial class UnityContainerExtension : IServiceCollectionAware
    {
        public IServiceProvider CreateServiceProvider()
        {
            return Instance.BuildServiceProvider();
        }

        public void Populate(IServiceCollection services)
        {
            Instance.Populate(services);
        }
    }
}
