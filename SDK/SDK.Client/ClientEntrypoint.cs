using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FreeFramework.SDK.Client.Communication;
using FreeFramework.SDK.Client.Runtime;
using FreeFramework.SDK.Client.Storage;
using JetBrains.Annotations;

namespace FreeFramework.SDK.Client;

[UsedImplicitly]
public sealed class ClientEntrypoint : ClientScript {
    public EventHandlerDictionary EntryEventHandlers => EventHandlers;
    public ExportDictionary EntryExports => Exports;
    public StateBag EntryGlobalState => GlobalState;
    public Player EntryLocalPlayer => LocalPlayer;
    public PlayerList EntryPlayers => Players;

    private HashSet<IClientModule> Modules { get; } = [];

    public ClientEntrypoint() {
        ClientGlobals.Entrypoint = this;
        ClientGlobals.StorageProvider = new ClientStorageProvider();
        ClientGlobals.EventBus = new ClientEventBus(this);
        ClientGlobals.RawEventBus = new ClientRawEventBus(this);
        ClientGlobals.NetworkEventBus = new ClientNetworkEventBus(this);
        ClientGlobals.RawNetworkEventBus = new ClientRawNetworkEventBus(this);

        EventHandlers["onClientResourceStart"] += (string resourceName) => {
            if (resourceName != API.GetCurrentResourceName()) {
                return;
            }
            
            LoadAllModules();
        };
    }

    private void LoadAllModules() {
        // Get types from the current app domain that inherit from IServerModule
        Type[] types = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(GetLoadableTypes)
                                .Where(t =>
                                    t is { IsAbstract: false, IsInterface: false } &&
                                    typeof(IClientModule).IsAssignableFrom(t)
                                ).ToArray();

        // Construct each module; adds all instances to a modules set
        foreach (Type t in types) {
            Debug.WriteLine($"Starting module {t.FullName}");
            Modules.Add((Activator.CreateInstance(t) as IClientModule)!);
        }

        // When the resource stops, unload all modules
        EventHandlers["onClientResourceStop"] += (string resourceName) => {
            if (resourceName != API.GetCurrentResourceName()) {
                return;
            }

            foreach (IClientModule module in Modules) {
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