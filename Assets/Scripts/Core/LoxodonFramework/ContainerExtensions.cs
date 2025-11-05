using ProjectBase.UI;

namespace VContainer
{
    public static class ContainerExtensions
    {
        public static T CreateViewModel<T>(this IObjectResolver resolver, bool needScope = false) where T : DIViewModelBase, new()
        {
            var vm = new T();

            IObjectResolver container = resolver;
            if (needScope)
            {
                container = resolver.CreateScope(builder => vm.Configure(builder));
            }

            container.Inject(vm);

            return vm;
        }
    }
}