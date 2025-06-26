// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPSoftworks.ChangeCaseExtension.Helpers
{
    internal static class StringHelper
    {
        public static string[] ToLines(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return [];
            }

            return input.Split(["\r\n", "\n", "\r"], StringSplitOptions.None | StringSplitOptions.TrimEntries);
        }
    }
}