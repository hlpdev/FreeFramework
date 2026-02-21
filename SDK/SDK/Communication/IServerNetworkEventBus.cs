using System;
using System.Collections.Generic;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Network event bus used for sending and receiving typed events to and from players.
///
/// Server -> Client(s)
/// Client -> Server
/// </summary>
public interface IServerNetworkEventBus {
    /// <summary>
    /// Publishes an event to a single player
    /// </summary>
    /// <param name="playerId">The player's ID</param>
    /// <param name="evt">The event object</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Publish<T>(int playerId, T evt) where T : IBusObject;
    
    /// <summary>
    /// Publishes an event to a specific set of players
    /// </summary>
    /// <param name="playerIds">An enumerable set of players</param>
    /// <param name="evt">The event object</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Publish<T>(IEnumerable<int> playerIds, T evt) where T : IBusObject;
    
    /// <summary>
    /// Publishes an event to all players
    /// </summary>
    /// <param name="evt">The event object</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Publish<T>(T evt) where T : IBusObject;

    /// <summary>
    /// Subscribes an action to a specific type
    /// </summary>
    /// <param name="callback">The action to bind the event to</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Subscribe<T>(Action<int, T> callback) where T : IBusObject;
}