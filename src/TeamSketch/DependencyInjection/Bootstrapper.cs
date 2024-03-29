﻿using Splat;
using TeamSketch.Services;

namespace TeamSketch.DependencyInjection;

public static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services)
    {
        RegisterServices(services);
    }

    private static void RegisterServices(IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IAppState>(() => new AppState());
    }
}
