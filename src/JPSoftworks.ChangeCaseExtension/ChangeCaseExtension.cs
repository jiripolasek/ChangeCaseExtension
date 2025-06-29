// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.InteropServices;

namespace JPSoftworks.ChangeCaseExtension;

[Guid("8f1b25eb-cc07-466b-9f98-ca8c73d7d1ed")]
public sealed partial class ChangeCaseExtension : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;

    private readonly ChangeCaseExtensionCommandsProvider _provider = new();

    public ChangeCaseExtension(ManualResetEvent extensionDisposedEvent)
    {
        this._extensionDisposedEvent = extensionDisposedEvent;
    }

    public object? GetProvider(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Commands => this._provider,
            _ => null,
        };
    }

    public void Dispose() => this._extensionDisposedEvent.Set();
}