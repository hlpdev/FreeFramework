using FreeFramework.SDK.Communication;
using FreeFramework.SDK.Storage;

namespace FreeFramework.SDK.Server;

public static class ServerGlobals {
    public static ServerEntrypoint Entrypoint { get; internal set; } = null!;
    public static IStorageProvider StorageProvider { get; internal set; } = null!;
    public static IEventBus EventBus { get; internal set; } = null!;
    public static IRawEventBus RawEventBus { get; internal set; } = null!;
    public static IServerNetworkEventBus NetworkEventBus { get; internal set; } = null!;
    public static IServerRawNetworkEventBus RawNetworkEventBus { get; internal set; } = null!;
}