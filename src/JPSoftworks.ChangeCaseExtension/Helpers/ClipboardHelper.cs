// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using Windows.System;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal static class ClipboardHelperEx
{
    // Function to send a single key event
    private static void SendSingleKeyboardInput(short keyCode, uint keyStatus)
    {
        var ignoreKeyEventFlag = (UIntPtr)0x5555;

        var inputShift = new NativeMethods.INPUT
        {
            type = NativeMethods.INPUTTYPE.INPUT_KEYBOARD,
            data = new NativeMethods.InputUnion
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wVk = keyCode,
                    dwFlags = keyStatus,

                    // Any keyevent with the extraInfo set to this value will be ignored by the keyboard hook and sent to the system instead.
                    dwExtraInfo = ignoreKeyEventFlag,
                },
            },
        };

        var inputs = new NativeMethods.INPUT[] { inputShift };
        _ = NativeMethods.SendInput(1, inputs, NativeMethods.INPUT.Size);
    }

    internal static void SendPasteKeyCombination()
    {
        ExtensionHost.LogMessage(new LogMessage() { Message = "Sending paste keys..." });

        SendSingleKeyboardInput((short)VirtualKey.LeftControl, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.RightControl, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.LeftWindows, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.RightWindows, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.LeftShift, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.RightShift, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.LeftMenu, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.RightMenu, (uint)NativeMethods.KeyEventF.KeyUp);

        // Send Ctrl + V
        SendSingleKeyboardInput((short)VirtualKey.Control, (uint)NativeMethods.KeyEventF.KeyDown);
        SendSingleKeyboardInput((short)VirtualKey.V, (uint)NativeMethods.KeyEventF.KeyDown);
        SendSingleKeyboardInput((short)VirtualKey.V, (uint)NativeMethods.KeyEventF.KeyUp);
        SendSingleKeyboardInput((short)VirtualKey.Control, (uint)NativeMethods.KeyEventF.KeyUp);

        ExtensionHost.LogMessage(new LogMessage() { Message = "Paste sent" });
    }
}