// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Buffers;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class CaseTransformers
{
    private static readonly SearchValues<char> SentenceEndings = SearchValues.Create('.', '!', '?');



    internal static string ToCapitalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return string.Create(input.Length, input, static (span, source) =>
        {
            var sourceSpan = source.AsSpan();

            for (var i = 0; i < sourceSpan.Length; i++)
            {
                var current = sourceSpan[i];

                if (char.IsLetter(current))
                {
                    var isWordStart = i == 0 || !char.IsLetter(sourceSpan[i - 1]);
                    span[i] = isWordStart
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

        return string.Create(input.Length, input, static (span, source) =>
        {
            var sourceSpan = source.AsSpan();

            span[0] = char.ToLowerInvariant(sourceSpan[0]);

            if (sourceSpan.Length > 1)
            {
                sourceSpan[1..].CopyTo(span[1..]);
            }
        });
    }



    internal static string ToRandomCase(string input)
    {
        return ToRandomCase(input, Random.Shared);
    }



    internal static string ToRandomCase(string input, Random random)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return string.Create(input.Length, (input, random), static (span, state) =>
        {
            var sourceSpan = state.input.AsSpan();
            var random = state.random;

            for (var i = 0; i < sourceSpan.Length; i++)
            {
                var c = sourceSpan[i];
                span[i] = random.Next(2) == 0
                    ? char.ToLowerInvariant(c)
                    : char.ToUpperInvariant(c);
            }
        });
    }



    internal static string ToSentenceCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return string.Create(input.Length, input, static (span, source) =>
        {
            var sourceSpan = source.AsSpan();

            for (var i = 0; i < sourceSpan.Length; i++)
            {
                var current = sourceSpan[i];

                if (char.IsLetter(current))
                {
                    var isSentenceStart = i == 0 || IsSentenceStart(sourceSpan, i);
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



    internal static string ToSwapCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return string.Create(input.Length, input, static (span, source) =>
        {
            var sourceSpan = source.AsSpan();

            for (var i = 0; i < sourceSpan.Length; i++)
            {
                var c = sourceSpan[i];
                span[i] = char.IsUpper(c)
                    ? char.ToLowerInvariant(c)
                    : char.ToUpperInvariant(c);
            }
        });
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

        return string.Create(input.Length, input, static (span, source) =>
        {
            var sourceSpan = source.AsSpan();

            span[0] = char.ToUpperInvariant(sourceSpan[0]);

            if (sourceSpan.Length > 1)
            {
                sourceSpan[1..].CopyTo(span[1..]);
            }
        });
    }



    private static bool IsSentenceStart(ReadOnlySpan<char> input, int currentIndex)
    {
        var lookBehind = input[..currentIndex];
        var lastSentenceEnd = lookBehind.LastIndexOfAny(SentenceEndings);

        if (lastSentenceEnd == -1)
        {
            return !lookBehind.ContainsAnyLetters();
        }

        var afterSentenceEnd = lookBehind[(lastSentenceEnd + 1)..];
        return !afterSentenceEnd.ContainsAnyLetters();
    }



    private static bool ContainsAnyLetters(this ReadOnlySpan<char> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            if (char.IsLetter(span[i]))
            {
                return true;
            }
        }

        return false;
    }
}