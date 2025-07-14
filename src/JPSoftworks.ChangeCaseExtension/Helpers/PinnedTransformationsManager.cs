// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal sealed class PinnedTransformationsManager : SettingsManagerBase<StorageModel>
{
    private static readonly Lazy<PinnedTransformationsManager> _instance = new(() => new());
    public static PinnedTransformationsManager Instance => _instance.Value;

    public event EventHandler<TransformationType>? PinnedChanged;
    private readonly List<TransformationType> _pinned = [];

    protected override JsonSerializerOptions JsonOptions => 
        StorageSourceGenerationContext.Default.Options;

    private PinnedTransformationsManager() : base(GetJsonPath())
    {
        this.LoadSettings();
    }

    public IReadOnlyList<TransformationType> Pinned
    {
        get
        {
            lock (this._pinned) 
            { 
                return this._pinned.AsReadOnly(); 
            }
        }
    }

    protected override StorageModel CreateModel()
    {
        lock (this._pinned)
        {
            return new() { Pinned = [..this._pinned] };
        }
    }

    protected override void ApplyModel(StorageModel model)
    {
        lock (this._pinned)
        {
            this._pinned.Clear();
            this._pinned.AddRange(model.Pinned);
        }
    }

    protected override void ClearState()
    {
        lock (this._pinned)
        {
            this._pinned.Clear();
        }
    }

    public void Pin(TransformationType transformationType)
    {
        var added = false;
        lock (this._pinned)
        {
            if (!this._pinned.Contains(transformationType))
            {
                this._pinned.Add(transformationType);
                added = true;
            }
        }

        if (added)
        {
            this.PinnedChanged?.Invoke(this, transformationType);
            // Fire and forget async save
            _ = Task.Run(async () => await this.SaveSettingsAsyncInternal());
        }
    }

    public void Unpin(TransformationType transformationType)
    {
        bool removed;
        lock (this._pinned)
        {
            removed = this._pinned.Remove(transformationType);
        }

        if (removed)
        {
            this.PinnedChanged?.Invoke(this, transformationType);
            // Fire and forget async save
            _ = Task.Run(async () => await this.SaveSettingsAsyncInternal());
        }
    }

    public bool IsPinned(TransformationType transformationType)
    {
        lock (this._pinned)
        {
            return this._pinned.Contains(transformationType);
        }
    }

    private static string GetJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "favorites.json");
    }

}

internal class StorageModel
{
    public List<TransformationType> Pinned { get; set; } = [];
}


[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(TransformationType))]
[JsonSerializable(typeof(StorageModel))]
internal sealed partial class StorageSourceGenerationContext : JsonSerializerContext
{
}