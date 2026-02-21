using System;
using System.Collections.Generic;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Network event bus used for sending and receiving non-typed events to and from players.
///
/// Server -> Client(s)
/// Client -> Server
/// </summary>
public interface IServerRawNetworkEventBus {
    /// <summary>
    /// Publishes an event to a specific player
    /// </summary>
    /// <param name="playerId">The player's ID</param>
    /// <param name="channel">The channel name</param>
    /// <param name="args">Objects to send</param>
    void Publish(int playerId, string channel, params object[] args);
    
    /// <summary>
    /// Publishes an event to a specific set of players
    /// </summary>
    /// <param name="playerIds">An enumerable set of players</param>
    /// <param name="channel">The channel name</param>
    /// <param name="args">Objects to send</param>
    void Publish(IEnumerable<int> playerIds, string channel, params object[] args);
    
    /// <summary>
    /// Publishes an event to all players
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