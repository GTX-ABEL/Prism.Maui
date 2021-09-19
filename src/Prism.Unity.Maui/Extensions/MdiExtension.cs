using Unity.Lifetime;

namespace Prism.Unity.Extensions
{
    internal class MdiExtension : global::Unity.Extension.UnityContainerExtension
    {
        protected override void Initialize()
        {
        }

        public ILifetimeContainer Lifetime => Context.Lifetime;
    }
}
