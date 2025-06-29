// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal sealed class HistoryManager
{
    public event EventHandler<TransformationType>? HistoryChanged;
    private readonly List<TransformationType> _history = new();

    public IReadOnlyList<TransformationType> History
    {
        get
        {
            lock (this._history) { return this._history.AsReadOnly(); }
        }
    }

    public void RememberTransformation(TransformationType transformationType)
    {
        lock (this._history)
        {
            this._history.Remove(transformationType);
            this._history.Insert(0, transformationType);
        }

        this.HistoryChanged?.Invoke(this, transformationType);
    }
}