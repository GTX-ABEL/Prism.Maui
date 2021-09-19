using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Prism.Unity.Maui.Lifetimes;
using Unity;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Prism.Unity.Extensions
{
    internal static class MicrosoftUnityExtensions
    {
        internal static IUnityContainer Populate(this IUnityContainer container, IServiceCollection services)
        {
            var lifetime = ((UnityContainer)container).Configure<MdiExtension>().Lifetime;
            var registerFunc = container.Register();

            container.SetRegister((type, name, registration) => container.AppendNew(type, name, registration));

            foreach (var descriptor in services)
                container.Register(descriptor, lifetime);

            container.SetRegister(registerFunc);

            return container;
        }

        internal static IServiceProvider BuildServiceProvider(this IUnityContainer container)
        {
            return new ServiceProvider.ServiceProvider(container);
        }

        private static IPolicySet AppendNew(this IUnityContainer container, Type type, string name, InternalRegistration registration)
        {
            var methodInfo = container.GetType()
                .GetMethod("AppendNew", BindingFlags.Instance | BindingFlags.NonPublic);

            if (methodInfo is null)
                return default;

            return (IPolicySet)methodInfo.Invoke(container, new object[] { type, name, registration });
        }

        private static void SetRegister(this IUnityContainer container, Func<Type, string, InternalRegistration, IPolicySet> registermethod)
        {
            var propInfo = container.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.Name == "Register");

            if (propInfo is null)
                return;

            propInfo.SetMethod.Invoke(container, new object[] { registermethod });
        }

        private static Func<Type, string, InternalRegistration, IPolicySet> Register(this IUnityContainer container)
        {
            var propInfo = container.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.Name == "Register");

            if (propInfo is null)
                return (t, s, r) => default;

            var value = propInfo.GetValue(container);
            return (Func<Type, string, InternalRegistration, IPolicySet>)value;
        }

        private static void Register(this IUnityContainer container,
            ServiceDescriptor serviceDescriptor, ILifetimeContainer lifetime)
        {
            if (serviceDescriptor.ImplementationType != null)
            {
                var name = serviceDescriptor.ServiceType.IsGenericTypeDefinition ? UnityContainer.All : null;
                container.RegisterType(serviceDescriptor.ServiceType,
                                       serviceDescriptor.ImplementationType,
                                       name,
                                       (ITypeLifetimeManager)serviceDescriptor.GetLifetime(lifetime));
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                container.RegisterFactory(serviceDescriptor.ServiceType,
                                        null,
                                        scope =>
                                        {
                                            var serviceProvider = scope.Resolve<IServiceProvider>();
                                            var instance = serviceDescriptor.ImplementationFactory(serviceProvider);
                                            return instance;
                                        },
                                       (IFactoryLifetimeManager)serviceDescriptor.GetLifetime(lifetime));
            }
            else if (serviceDescriptor.ImplementationInstance != null)
            {
                container.RegisterInstance(serviceDescriptor.ServiceType,
                                           null,
                                           serviceDescriptor.ImplementationInstance,
                                           (IInstanceLifetimeManager)serviceDescriptor.GetLifetime(lifetime));
            }
            else
            {
                throw new InvalidOperationException("Unsupported registration type");
            }
        }

        private static LifetimeManager GetLifetime(this ServiceDescriptor serviceDescriptor, ILifetimeContainer lifetime)
        {
            switch (serviceDescriptor.Lifetime)
            {
                case ServiceLifetime.Scoped:
                    return new HierarchicalLifetimeManager();
                case ServiceLifetime.Singleton:
                    return new InjectionSingletonLifetimeManager(lifetime);
                case ServiceLifetime.Transient:
                    return new InjectionTransientLifetimeManager();
                default:
                    throw new NotImplementedException(
                        $"Unsupported lifetime manager type '{serviceDescriptor.Lifetime}'");
            }
        }
    }
}
