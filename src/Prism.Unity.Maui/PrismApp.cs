using Unity;

namespace Prism.Unity.Maui
{
    public class PrismApp
    {
        public static PrismAppBuilder CreateBuilder()
        {
            return new UnityPrismAppBuilder();
        }

        public static PrismAppBuilder CreateBuilder(IUnityContainer container)
        {
            return new UnityPrismAppBuilder(container);
        }
    }
}
