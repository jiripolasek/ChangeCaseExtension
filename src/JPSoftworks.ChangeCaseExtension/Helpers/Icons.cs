// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal static class Icons
{
    public static IconInfo Copy { get; } = new("\uE8C8");
    public static IconInfo History { get; } = new("\ue823");
    public static IconInfo Refresh { get; } = new("\ue777");
    public static IconInfo Replace { get; } = new("\ue845");

    public static IconInfo ChangeCaseIcon { get; }
        = IconHelpers.FromRelativePath("Assets\\Square44x44Logo.targetsize-40_altform-unplated.png");
}