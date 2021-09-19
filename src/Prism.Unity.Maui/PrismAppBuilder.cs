using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public class UnityPrismAppBuilder : PrismAppBuilder
    {
        public UnityPrismAppBuilder()
        {
        }

        public UnityPrismAppBuilder(IUnityContainer container)
            : base(new UnityContainerExtension(container))
        {
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }
    }
}
