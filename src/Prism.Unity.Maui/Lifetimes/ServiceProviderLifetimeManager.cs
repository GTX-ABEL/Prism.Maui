using System;
using Unity.Lifetime;

namespace Prism.Unity.Maui.Lifetimes
{
    internal class ServiceProviderLifetimeManager : LifetimeManager, IInstanceLifetimeManager
    {
        IServiceProvider _serviceProvider;

        public ServiceProviderLifetimeManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object GetValue(ILifetimeContainer container = null)
        {
            return _serviceProvider;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            throw new NotImplementedException();
        }
    }
}
