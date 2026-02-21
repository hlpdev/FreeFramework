using System;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Event bus for sending non-typed events locally.
///
/// Server -> Server
/// Client -> Client
/// </summary>
public interface IRawEventBus {
    /// <summary>
    /// Publishes an event locally
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