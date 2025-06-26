// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed record TransformationDefinition(
    TransformationType Type,
    string? Title,
    Func<string, string> Transform)
{
    public string GetDisplayTitle()
    {
        return this.Title ?? this.Type switch
        {
            TransformationType.TitleCase => $"Title Case ({Thread.CurrentThread.CurrentCulture.DisplayName})",
            _ => this.Type.ToString()
        };
    }
}