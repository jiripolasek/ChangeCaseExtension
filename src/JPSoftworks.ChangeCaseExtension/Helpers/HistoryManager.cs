// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.ChangeCaseExtension.Pages;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal sealed class HistoryManager
{
    private readonly List<TransformationType> _history = new();

    public IReadOnlyList<TransformationType> History => this._history.AsReadOnly();

    public void RememberTransformation(TransformationType transformationType)
    {
        this._history.Remove(transformationType);
        this._history.Insert(0, transformationType);
    }
}