using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        ServerGlobals.Entrypoint = this;
        ServerGlobals.StorageProvider = new ServerStorageProvider(API.GetConvar("ff_database_connection", ""));
        ServerGlobals.EventBus = new ServerEventBus(this);
        ServerGlobals.RawEventBus = new ServerRawEventBus(this);
        ServerGlobals.NetworkEventBus = new ServerNetworkEventBus(this);
        ServerGlobals.RawNetworkEventBus = new ServerRawNetworkEventBus(this);

        EventHandlers["onResourceStart"] += (string resourceName) => {
            if (resourceName != API.GetCurrentResourceName()) {
                return;
            }
            
            LoadAllModules();
        };
    }

    public void BindToTick(Func<Task> callback) {
        Tick += callback;
    }

    private void LoadAllModules() {
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
            Modules.Add((Activator.CreateInstance(t) as IServerModule)!);
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