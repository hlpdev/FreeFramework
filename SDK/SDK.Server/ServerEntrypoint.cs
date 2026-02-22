using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FreeFramework.SDK.Server.Communication;
using FreeFramework.SDK.Server.Runtime;
using FreeFramework.SDK.Server.Storage;
using JetBrains.Annotations;

namespace FreeFramework.SDK.Server;

[UsedImplicitly]
public sealed class ServerEntrypoint : ServerScript {
    public EventHandlerDictionary EntryEventHandlers => EventHandlers;
    public ExportDictionary EntryExports => Exports;
    public StateBag EntryGlobalState => GlobalState;
    public PlayerList EntryPlayers => Players;

    private HashSet<IServerModule> Modules { get; } = [];

    public ServerEntrypoint() {
        var services = new ServiceCollection();

        // Add entrypoint citizen fx script as a singleton
        services.AddSingleton(this);
        
        // Initialize storage services
        services.AddSingleton<IStorageProvider, ServerStorageProvider>(
            _ => new ServerStorageProvider(API.GetConvar("ff_database_connection", ""))
        );

        // Initialize communication services
        services.AddSingleton<IEventBus, ServerEventBus>();
        services.AddSingleton<IRawEventBus, ServerRawEventBus>();
        services.AddSingleton<IServerNetworkEventBus, ServerNetworkEventBus>();
        services.AddSingleton<IServerRawNetworkEventBus, ServerRawNetworkEventBus>();

        // Create the service provider
        IServiceProvider provider = services.BuildServiceProvider();

        // Get types from the current app domain that inherit from IServerModule
        Type[] types = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(GetLoadableTypes)
                                .Where(t =>
                                    t is { IsAbstract: false, IsInterface: false } &&
                                    typeof(IServerModule).IsAssignableFrom(t)
                                ).ToArray();

        // Construct each module and run dependency injection; adds all instances to a modules set
        foreach (Type t in types) {
            Debug.WriteLine($"Starting module {t.FullName}");
            Modules.Add((IServerModule)ActivatorUtilities.CreateInstance(provider, t));
        }

        // When the resource stops, unload all modules
        EventHandlers["onResourceStop"] += (string resourceName) => {
            if (resourceName != API.GetCurrentResourceName()) {
                return;
            }

            foreach (IServerModule module in Modules) {
                module.Dispose();
            }

            Modules.Clear();
        };
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
        try {
            return assembly.GetTypes();
        } catch (ReflectionTypeLoadException e) {
            return e.Types.Where(t => t != null);
        }
    }
}