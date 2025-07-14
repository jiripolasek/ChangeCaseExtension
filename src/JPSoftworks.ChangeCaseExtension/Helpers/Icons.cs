// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal static class Icons
{
    public static IconInfo Copy { get; } = new("\uE8C8");
    public static IconInfo History { get; } = new("\ue823");
    public static IconInfo Refresh { get; } = new("\ue777");
    public static IconInfo Replace { get; } = new("\ue845");

    public static IconInfo FontSerif { get; } = new("\ue8d2");
    public static IconInfo FontSansSerif { get; } = new("\ue8e9");
    public static IconInfo Account { get; } = new("\ue910");
    public static IconInfo Text { get; } = new("\ue8a5");
    public static IconInfo TextCase { get; } = new("\ue8a6");
    public static IconInfo Code { get; } = new("\ue943");
    public static IconInfo Keyboard { get; } = new("\ue765");

    public static IconInfo Space { get; } = new("\ue75d");

    public static IconInfo View { get; } = new("\ue890");

    public static IconInfo Broom { get; } = IconHelpers.FromRelativePaths(
        @"Assets\Icons\ic_fluent_broom_24_regular_light.svg",
        @"Assets\Icons\ic_fluent_broom_24_regular_dark.svg");

    public static IconInfo Pin { get; } = new("\uE718");
    public static IconInfo Unpin { get; } = new("\uE77a");

    public static IconInfo ChangeCaseIcon { get; }
        = IconHelpers.FromRelativePath("Assets\\Square44x44Logo.targetsize-40_altform-unplated.png");
}