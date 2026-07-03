using System.Collections.Generic;
using VContainer;
using Whaledevelop.Services;

namespace Game
{
    public static class GameServicesComposition
    {
        public static void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<global::Game.UGUIService>(Lifetime.Singleton).AsSelf().As<global::Game.IUGUIService>().As<IService>();

            return;
        }

        public static IReadOnlyList<IService> ResolveServices(IObjectResolver resolver)
        {
            var services = new IService[]
            {
                resolver.Resolve<global::Game.UGUIService>(),
            };

            return services;
        }
    }
}
