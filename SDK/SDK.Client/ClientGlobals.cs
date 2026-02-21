using FreeFramework.SDK.Communication;
using FreeFramework.SDK.Storage;

namespace FreeFramework.SDK.Client;

public static class ClientGlobals {
    public static ClientEntrypoint Entrypoint { get; internal set; } = null!;
    public static IStorageProvider StorageProvider { get; internal set; } = null!;
    public static IEventBus EventBus { get; internal set; } = null!;
    public static IRawEventBus RawEventBus { get; internal set; } = null!;
    public static IClientNetworkEventBus NetworkEventBus { get; internal set; } = null!;
    public static IClientRawNetworkEventBus RawNetworkEventBus { get; internal set; } = null!;
}