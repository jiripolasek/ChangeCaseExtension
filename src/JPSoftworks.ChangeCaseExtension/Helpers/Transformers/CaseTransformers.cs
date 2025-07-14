// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Buffers;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class CaseTransformers
{
    internal static string ToCapitalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var result = new char[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            char current = input[i];

            if (char.IsLetter(current))
            {
                bool isWordStart = i == 0 || !char.IsLetter(input[i - 1]);

                result[i] = isWordStart
                    ? char.ToUpperInvariant(current)
                    : char.ToLowerInvariant(current);
            }
            else
            {
                result[i] = current;
            }
        }

        return new(result);
    }



    internal static string ToLowerCase(string input)
    {
        return input.ToLowerInvariant();
    }



    internal static string ToLowerFirst(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return char.ToLowerInvariant(input[0]) + input[1..];
    }



    internal static string ToRandomCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var random = new Random();
        return string.Concat(input.Select(c => random.Next(2) == 0 ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }



    private static readonly SearchValues<char> SentenceEndings = SearchValues.Create(['.', '!', '?']);



    internal static string ToSentenceCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return string.Create(input.Length, input, static (span, source) =>
        {
            ReadOnlySpan<char> sourceSpan = source.AsSpan();

            for (int i = 0; i < sourceSpan.Length; i++)
            {
                char current = sourceSpan[i];

                if (char.IsLetter(current))
                {
                    bool isSentenceStart = i == 0 || IsSentenceStart(sourceSpan, i);

                    span[i] = isSentenceStart
                        ? char.ToUpperInvariant(current)
                        : char.ToLowerInvariant(current);
                }
                else
                {
                    span[i] = current;
                }
            }
        });
    }



    private static bool IsSentenceStart(ReadOnlySpan<char> input, int currentIndex)
    {
        var lookBehind = input[..currentIndex];

        int lastSentenceEnd = lookBehind.LastIndexOfAny(SentenceEndings);

        if (lastSentenceEnd == -1)
        {
            return !lookBehind.ContainsAnyLetters();
        }

        var afterSentenceEnd = lookBehind[(lastSentenceEnd + 1)..];
        return !afterSentenceEnd.ContainsAnyLetters();
    }



    private static bool ContainsAnyLetters(this ReadOnlySpan<char> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (char.IsLetter(span[i]))
            {
                return true;
            }
        }

        return false;
    }



    internal static string ToSwapCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return string.Concat(input.Select(static c => char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }



    internal static string ToUpperCase(string input)
    {
        return input.ToUpperInvariant();
    }



    internal static string ToUpperFirst(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return char.ToUpperInvariant(input[0]) + input[1..];
    }
}