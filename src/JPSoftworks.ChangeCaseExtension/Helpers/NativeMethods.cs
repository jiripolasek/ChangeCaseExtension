// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local", Justification = "Allow use of Win32 naming conventions")]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Allow use of Win32 naming conventions")]
internal static partial class NativeMethods
{
    internal const uint SHUTDOWN_NORETRY = 0x00000001;

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);   
}