using System;

namespace FreeFramework.SDK.Runtime;

public interface IModule : IDisposable {
    public string Name { get; }
}