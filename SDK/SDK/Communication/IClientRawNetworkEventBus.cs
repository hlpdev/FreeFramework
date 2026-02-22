using System;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Network event bus used for sending and receiving non-typed events to and from the server.
///
/// Server -> Client
/// Client -> Server
/// </summary>
public interface IClientRawNetworkEventBus {
    /// <summary>
    /// Publishes an event to the server
    /// </summary>
    /// <param name="channel">The channel name</param>
    /// <param name="args">Objects to send</param>
    void Publish(string channel, params object[] args);

    /// <summary>
    /// Subscribes a delegate to a specific channel
    /// </summary>
    /// <param name="channel">The channel name</param>
    /// <param name="callback">The delegate to bind the event to</param>
    void Subscribe(string channel, Delegate callback);
}